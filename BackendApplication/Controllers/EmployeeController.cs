using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using BackendApplication.Models;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace BackendApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnv;
        public EmployeeController(IConfiguration configuration, IWebHostEnvironment webHostEnv)
        {
            _configuration = configuration;
            _webHostEnv = webHostEnv;
        }

        [HttpGet]
        public JsonResult Get()
        {
            string query = @"select EmployeeId, EmployeeName, Department, 
            convert(varchar(10), DateOfJoining, 120) as DateOfJoining, PhotoFileName from dbo.Employee";
            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("EmployeeAppCon");
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myComm = new SqlCommand(query, myCon))
                {
                    myReader = myComm.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }
            return new JsonResult(table);
        }

        [HttpPost]
        public JsonResult Post(Employee emp)
        {
            string query = @"insert into dbo.Employee (EmployeeName, Department,DateOfJoining, PhotoFileName) values (@EmployeeName, @Department, @DateOfJoining, @PhotoFileName)";
            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("EmployeeAppCon");
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myComm = new SqlCommand(query, myCon))
                {
                    myComm.Parameters.AddWithValue("@EmployeeName", emp.EmployeeName);
                    myComm.Parameters.AddWithValue("@Department", emp.Department);
                    myComm.Parameters.AddWithValue("@DateOfJoining", emp.DateOfJoining);
                    myComm.Parameters.AddWithValue("@PhotoFileName", emp.PhotoFileName);
                    myReader = myComm.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }
            return new JsonResult("Added Successfully");
        }

        [HttpPut]
        public JsonResult Put(Employee emp)
        {
            string query = @"update dbo.Employee set EmployeeName = @EmployeeName, Department=@Department, DateOfJoining=@DateOfJoining, PhotoFileName=@PhotoFileName where EmployeeId=@EmployeeId";
            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("EmployeeAppCon");
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myComm = new SqlCommand(query, myCon))
                {
                    myComm.Parameters.AddWithValue("@EmployeeName", emp.EmployeeName);
                    myComm.Parameters.AddWithValue("@EmployeeId", emp.EmployeeId);
                    myComm.Parameters.AddWithValue("@Department", emp.Department);
                    myComm.Parameters.AddWithValue("@DateOfJoining", emp.DateOfJoining);
                    myComm.Parameters.AddWithValue("@PhotoFileName", emp.PhotoFileName);
                    myReader = myComm.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }
            return new JsonResult("Updated Successfully");
        }

        [HttpDelete("{id}")]
        public JsonResult Delete(int id)
        {
            string query = @"delete from dbo.Employee where EmployeeId=@EmployeeId";
            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("EmployeeAppCon");
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myComm = new SqlCommand(query, myCon))
                {
                    myComm.Parameters.AddWithValue("@EmployeeId", id);
                    myReader = myComm.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }
            return new JsonResult("Deleted Successfully");
        }


        [Route("SaveFile")]
        [HttpPost]
        public JsonResult SaveFile()
        {
            try
            {
                var httpRequest = Request.Form;
                var postedFile = httpRequest.Files[0];
                string filename = postedFile.FileName;
                var physicalPath = _webHostEnv.ContentRootPath + "/Photos/" + filename;

                using(var stream = new FileStream(physicalPath,FileMode.Create))
                {
                    postedFile.CopyTo(stream);
                }
                return new JsonResult(filename);
            }
            catch (Exception)
            {
                return new JsonResult("anonymous.png");
            }
        }

    }
}
