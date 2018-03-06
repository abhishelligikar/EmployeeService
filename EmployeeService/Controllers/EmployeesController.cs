using EmployeeDataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Web.Http;
using System.Web.Http.Cors;

namespace EmployeeService.Controllers
{
    //[EnableCorsAttribute("*","*","*")]
    //[RequireHttps]
    [RoutePrefix("api/employees")]
    public class EmployeesController : ApiController
    {
        [HttpGet]
        [Route("loademployees")]
        public IEnumerable<Employee> loademployees()
        {
            using (EmployeeDBEntities entities = new EmployeeDBEntities())
            {
                return entities.Employees.ToList();
            }
        }

        [Route("LoadEmployee/{id:int}")]
        [HttpGet]
        public HttpResponseMessage LoadEmployeeById(int id)
        {
            using (EmployeeDBEntities entities = new EmployeeDBEntities())
            {
                var entity = entities.Employees.FirstOrDefault(e => e.ID == id);
                if (entity != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, entity);
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound,
                        "Employee with Id " + id.ToString() + " not found");
                }
            }
        }

        [HttpPost]
        public HttpResponseMessage InsertEmployee([FromBody]Employee employee)
        {
            try
            {
                using (EmployeeDBEntities entities = new EmployeeDBEntities())
                {
                    entities.Employees.Add(employee);
                    entities.SaveChanges();

                    var message = Request.CreateResponse(HttpStatusCode.Created, employee);
                    message.Headers.Location = new Uri(Request.RequestUri + "/" + employee.ID.ToString());
                    return message;
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        [HttpDelete]
        public HttpResponseMessage RemoveEmployee(int id)
        {
            try
            {
                using (EmployeeDBEntities entities = new EmployeeDBEntities())
                {
                    Employee entry = entities.Employees.FirstOrDefault(e => e.ID == id);
                    if (entry != null)
                    {
                        entities.Employees.Remove(entry);
                        entities.SaveChanges();
                        return Request.CreateResponse(HttpStatusCode.OK, entry);
                    }
                    else
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.NotFound,
                            "Employee with id " + id.ToString() + " not found");
                    }
                }
            }
            catch (Exception)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Its a bad request, please try later");
            }

        }

        [HttpPut]
        public HttpResponseMessage UpdateEmployee(int id, [FromBody]Employee employee)
        {
            try
            {
                using (EmployeeDBEntities entities = new EmployeeDBEntities())
                {
                    Employee objemployee = entities.Employees.FirstOrDefault(e => e.ID == id);
                    if (objemployee != null)
                    {
                        objemployee.FirstName = employee.FirstName;
                        objemployee.LastName = employee.LastName;
                        objemployee.Gender = employee.Gender;
                        objemployee.salary = employee.salary;

                        entities.SaveChanges();
                        return Request.CreateResponse(HttpStatusCode.OK, objemployee);
                    }
                    else
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Employee with id : " + id.ToString() + " was not found");
                    }
                }
            }
            catch (Exception)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Its a bad request");
            }
        }


        public HttpResponseMessage Put([FromBody]int id, [FromUri]Employee employee)
        {
            try
            {
                using (EmployeeDBEntities entities = new EmployeeDBEntities())
                {
                    var entity = entities.Employees.FirstOrDefault(e => e.ID == id);
                    if (entity == null)
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.NotFound,
                            "Employee with Id " + id.ToString() + " not found to update");
                    }
                    else
                    {
                        entity.FirstName = employee.FirstName;
                        entity.LastName = employee.LastName;
                        entity.Gender = employee.Gender;
                        entity.salary = employee.salary;

                        entities.SaveChanges();

                        return Request.CreateResponse(HttpStatusCode.OK, entity);
                    }
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        [BasicAuthentication]
        [Route("loadonSign")]
        [HttpGet]
        public HttpResponseMessage LoadEmployeeOnSignIn()
        {
            try
            {
                string username = Thread.CurrentPrincipal.Identity.Name;

                using (EmployeeDBEntities entities = new EmployeeDBEntities())
                {
                    switch (username.ToLower())
                    {
                        case "male":
                            return Request.CreateResponse(HttpStatusCode.OK,
                                entities.Employees.Where(e => e.Gender.ToLower() == "male").ToList());
                        case "female":
                            return Request.CreateResponse(HttpStatusCode.OK,
                                entities.Employees.Where(e => e.Gender.ToLower() == "female").ToList());
                        default:
                            return Request.CreateResponse(HttpStatusCode.BadRequest);
                    }
                }

            }
            catch (Exception)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Internal server error");
            }
        }
    }
}
