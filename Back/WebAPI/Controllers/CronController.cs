using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SqlClient;
using WebAPI.Models;
using Hangfire;
using System.Net.NetworkInformation;
using System.Net;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CronController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public CronController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public JsonResult Get()
        {
            string query = @"
                    select Id,Name,Description,Url,CronExpression,IsActive,CronResult,LastExecution from dbo.CRON";
            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("AutoTasks");
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);

                    myReader.Close();
                    myCon.Close();
                }
            }

            return new JsonResult(table);
        }

        [HttpPost]
        public JsonResult Post(CRONExpression cron)
        {
            string query = @"insert into dbo.CRON (Name,Description,Url,CronExpression,IsActive) output INSERTED.ID values(@name,@description,@url,@cr,@isactive)";
            string sqlDataSource = _configuration.GetConnectionString("AutoTasks");
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                int Id = 0;
                using (SqlCommand cmd = new SqlCommand(query, myCon))
                {
                    cmd.Parameters.AddWithValue("@name", cron.Name);
                    cmd.Parameters.AddWithValue("@description", cron.Description);
                    cmd.Parameters.AddWithValue("@url", cron.Url);
                    cmd.Parameters.AddWithValue("@cr", cron.CronExpression);
                    cmd.Parameters.AddWithValue("@isactive", cron.IsActive);
                    myCon.Open();

                    cron.Id = (int)cmd.ExecuteScalar();

                    if (myCon.State == ConnectionState.Open)
                        myCon.Close();
                }
            }
            RecurringJob.AddOrUpdate<PingUrl>("PingUrl"+cron.Id.ToString(), x=> x.DoPing(cron),cron.CronExpression);
            return new JsonResult("Added Successfully");
        }

        [HttpPut]
        public JsonResult Put(CRONExpression cron)
        {
            string query = @"
                    update dbo.CRON set 
                    Name = '" + cron.Name + @"',
                    Description = '" + cron.Description + @"',
                    Url = '" + cron.Url + @"',
                    CronExpression = '" + cron.CronExpression + @"',
                    IsActive = '" + cron.IsActive + @"'
                    where Id = " + cron.Id + @" 
                    ";
            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("AutoTasks");
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);

                    myReader.Close();
                    myCon.Close();
                }
            }

            return new JsonResult("Updated Successfully");
        }

        [HttpDelete("{id}")]
        public JsonResult Delete(int Id)
        {
            string query = @"
                    delete from dbo.CRON
                    where Id = " + Id + @" 
                    ";
            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("AutoTasks");
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);

                    myReader.Close();
                    myCon.Close();
                }
            }
            RecurringJob.RemoveIfExists("PingUrl" + Id.ToString());
            return new JsonResult("Deleted Successfully");
        }
        
    }

    public class PingUrl
    {
        private readonly IConfiguration _configuration;
        public PingUrl(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        
        public void DoPing(CRONExpression cron)
        {
            string headers = "";
            try
            {
                Ping myPing = new Ping();
                cron.Url = cron.Url.Replace("http://", "").Replace("https://", "");
                PingReply reply = myPing.Send(cron.Url);
                if (reply.Status == IPStatus.Success)
                {
                    HttpWebRequest myHttpWebRequest = (HttpWebRequest)WebRequest.Create("http://" + cron.Url);
                    HttpWebResponse myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse();

                    for (int i = 0; i < myHttpWebResponse.Headers.Count; ++i)
                    {
                        headers += myHttpWebResponse.Headers.Keys[i] + ":" + myHttpWebResponse.Headers[i];
                    }
                    myHttpWebResponse.Close();

                    if (headers.Length > 1000)
                    {
                        headers = headers.Substring(0, 1000);
                    }
                    UpdateTasksResult(cron, headers);
                }
            }
            catch (Exception ex)
            {
                headers = "";
                Console.WriteLine("ERROR: " + ex.Message);
            }
        }

        public bool UpdateTasksResult(CRONExpression cron, string headers)
        {
            bool blnR = true;
            try
            {
                string query = @"
                    update dbo.CRON set 
                    CronResult = '" + headers + @"',
                    LastExecution = GETDATE()
                    where Id = " + cron.Id + @" 
                    ";
                DataTable table = new DataTable();
                string sqlDataSource = _configuration.GetConnectionString("AutoTasks");
                SqlDataReader myReader;
                using (SqlConnection myCon = new SqlConnection(sqlDataSource))
                {
                    myCon.Open();
                    using (SqlCommand myCommand = new SqlCommand(query, myCon))
                    {
                        myReader = myCommand.ExecuteReader();
                        table.Load(myReader); ;

                        myReader.Close();
                        myCon.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                blnR = false;
                Console.WriteLine("ERROR: " + ex.Message);
            }
            return blnR;
        }
    }
}
