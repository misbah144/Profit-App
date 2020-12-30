using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using DAP.Models;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using System.Data.Entity;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;
//using HiQPdf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web;
using System.Web.Mvc;
//using Export.Models;
//using System.Data.Linq;
using System.Web.Script.Serialization;
using System.Configuration;
using System.IO;
using System.Data.Entity;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Web.UI.HtmlControls;
using System.Text;
using iTextSharp;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.html.simpleparser;   
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions;
using CrystalDecisions.ReportAppServer.ClientDoc;
//using log4net;

namespace DAP.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            if (this.HttpContext.Session["username_session"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("login", "Home");
            }

        }
        public ActionResult edit_table()        //  works needs to be done for bank setup againt new db ************   
        {
            //if (this.HttpContext.Session["username_session"].ToString() != string.Empty)
            //{
            //    return View();
            //}
            //else
            //{
            //    return RedirectToAction("login", "Home");
            //}

            DataTable table12 = new DataTable();
            string constr1 = ConfigurationManager.ConnectionStrings["DAP_NEW"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr1))
            {
                string query = "select RATING_ID,RATING_NAME from config_rating";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    SqlDataAdapter ds = new SqlDataAdapter(cmd);
                    ds.Fill(table12);
                }
            }

            List<UserModel> r_list = new List<UserModel>();

            for (int i = 0; i < table12.Rows.Count; i++)
            {
                
                r_list.Add(new UserModel()
                {
                    rating = table12.Rows[i]["RATING_NAME"].ToString(),
                    rating_id = table12.Rows[i]["RATING_ID"].ToString(),
                    //depertment = table12.Rows[i]["DEPT_SHORTNAME"].ToString(),
                });
            }

            //UserLogin user1 = new UserLogin();
            //user1.dept_list = d_list;    

            DataTable table = new DataTable();
            string constr = ConfigurationManager.ConnectionStrings["DAP"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = "select bankid,bankname,shortname,rating from Banks order by bankid  ";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    SqlDataAdapter ds = new SqlDataAdapter(cmd);
                    ds.Fill(table);
                }
            }
            List<UserModel> users = new List<UserModel>();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                //  bankname="Anubhav",shortname="Chaudhary",rating="AA+"
                users.Add(new UserModel()
                {
                    bankid = table.Rows[i]["bankid"].ToString(),
                    bankname = table.Rows[i]["bankname"].ToString(),
                    shortname = table.Rows[i]["shortname"].ToString(),
                    rating = table.Rows[i]["rating"].ToString()
                });
            }
            int f = users.Count;
            //{  
            //    new UserModel (){ bankid="1", bankname="Anubhav",shortname="Chaudhary",rating="AA+" },   
            //    new UserModel (){ bankid="2", bankname="Mohit",shortname="Singh",rating="AA+" },  
            //    new UserModel (){ bankid="3", bankname="Sonu",shortname="Garg",rating="AA+" },  
            //    new UserModel (){ bankid="4", bankname="Shalini",shortname="Goel",rating="AA+" },  
            //    new UserModel (){ bankid="5", bankname="James",shortname="Bond",rating="AA+" },  
            //};
            //return users;
            UserModel user1 = new UserModel();
            user1.ulist = users;
            user1.r_list = r_list;
            return View(user1);
            //List<UserModel> users = UserModel.getUsers();
            //return View(users);
        }
        public JsonResult ChangeUser(UserModel model)
        {
            // Update model to your db  
            int s = 100;
            string bank_id = model.bankid.ToString();
            string bank_name = model.bankname.ToString();
            string bank_shortname = model.shortname.ToString();
            string bank_rating = model.rating.ToString();

            string upd_query = "update banks set ";
            upd_query = upd_query + " bankname =";
            upd_query = upd_query + "'" + bank_name + "'" + ",";
            upd_query = upd_query + "shortname =";
            upd_query = upd_query + "'" + bank_shortname + "'" + ",";
            upd_query = upd_query + "rating =";
            upd_query = upd_query + "'" + bank_rating + "'";
            upd_query = upd_query + " where bankid = " + bank_id;

            string constr = ConfigurationManager.ConnectionStrings["DAP"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                con.Open();                
                SqlCommand cmd = new SqlCommand(upd_query, con);    // to be corrected 
                // SqlCommand cmd1 = new SqlCommand("select top 1 * from Customer_IDs where ID_NO ='" + txtCnicNo + "'", oCnn);   
                s = cmd.ExecuteNonQuery();
                con.Close();
            }


            string message1 = "Successful";
            string message2 = "unsuccessful";
            if (s == 0)
            {
                return Json(message2, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(message1, JsonRequestBehavior.AllowGet);
            }

        }
        [HttpPost]
        public ActionResult insert_bank(UserModel model)
        {
            string user_session = this.HttpContext.Session["username_session"].ToString(); // retriving value from session
            //  string constr = string.Empty;
            var constr = ConfigurationManager.ConnectionStrings["DAP_NEW"].ConnectionString;
            int s = 100;
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = "INSERT INTO Banks(BANKID,bankname,shortname,username,rating) VALUES((select max(bankid)+1 from Banks),@bank_name,@short_name,@username,@rating)";
                //query += " SELECT SCOPE_IDENTITY()";
                using (SqlCommand cmd = new SqlCommand(query))
                {
                    cmd.Connection = con;
                    con.Open();
                    //cmd.Parameters.AddWithValue("@bank_id",model.bankid);
                    cmd.Parameters.AddWithValue("@bank_name", model.bankname);
                    cmd.Parameters.AddWithValue("@short_name", model.shortname);
                    //cmd.Parameters.AddWithValue("@username", model.username);
                    cmd.Parameters.AddWithValue("@username", user_session);
                    cmd.Parameters.AddWithValue("@rating", model.rating);
                    // cmd.Parameters.AddWithValue("@Country", customer.Country);
                    s = cmd.ExecuteNonQuery();
                    //customer.CustomerId = Convert.ToInt32(cmd.ExecuteScalar());
                    con.Close();
                }
            }

            //string constr = ConfigurationManager.ConnectionStrings["DAP_NEW"].ConnectionString;
            //using (SqlConnection con = new SqlConnection(constr))
            //{

            //    con.Open();
            //    // MessageBox.Show ("Connection Open ! ");
            //    // cnn.Close();
            //    SqlCommand cmd = new SqlCommand("SP_INSERT_BANK", con);    // to be corrected 
            //    // SqlCommand cmd1 = new SqlCommand("select top 1 * from Customer_IDs where ID_NO ='" + txtCnicNo + "'", oCnn);   
            //    cmd.CommandType = CommandType.StoredProcedure;
            //    //cmd.Parameters.AddWithValue("@Rating_Id", model.rating_id);
            //    cmd.Parameters.AddWithValue("@Rating_Name", model.rating_name);
            //    cmd.Parameters.AddWithValue("@Rating_ShortName", model.rating_shortname);
            //    cmd.Parameters.AddWithValue("@Rating_Status", model.rating_status);
            //    cmd.Parameters.AddWithValue("@Rating_IsDefault", model.rating_isdefault_bool);
            //    cmd.Parameters.AddWithValue("@sUser", user_session);
            //    s = cmd.ExecuteNonQuery();
            //    con.Close();
            //}

            string message1 = "";            
            if (s == 0)
            { message1 = "unSuccessful";
                //  return Json(message2, JsonRequestBehavior.AllowGet);
            }
            else
            { message1 = "Successful";
              //return Json(message1, JsonRequestBehavior.AllowGet);
            }

            return RedirectToAction("edit_table", "Home");
            // return View();
        }     //  for adding bank in banksetup screen  
        public ActionResult login(string emesg)               //get 
        {
            // string encrypted = Util.Encrypt("Today123");
            //   string my_session = String.Empty;

            UserLogin user1 = new UserLogin();
            this.HttpContext.Session["username_session"] = null;
            string my_session = null;
            // my_session= this.HttpContext.Session["username_session"].ToString();
            if (emesg != null)
            { user1.tcount = "ERROR";}

            DataTable table = new DataTable();
            string constr = ConfigurationManager.ConnectionStrings["DAP12"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = "select DEPT_ID,DEPT_SHORTNAME,DEPT_ISDEFAULT from CONFIG_DEPARTMENT order by DEPT_ISDEFAULT DESC";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    SqlDataAdapter ds = new SqlDataAdapter(cmd);
                    ds.Fill(table);
                }
            }

            List<UserLogin> d_list = new List<UserLogin>();
            for (int i = 0; i < table.Rows.Count; i++)
            {                //  bankname="Anubhav",shortname="Chaudhary",rating="AA+"
                d_list.Add(new UserLogin()
                {
                    depertment_id = table.Rows[i]["DEPT_ID"].ToString(),
                    depertment = table.Rows[i]["DEPT_SHORTNAME"].ToString(),
                });
            }
            user1.dept_list = d_list;

            //if (this.HttpContext.Session["username_session"] !="")
            //{
            //    user1.tcount = "ERROR";    
            //    return View(user1); 
            //}
            //else
            return View(user1);
        }
        [HttpPost]
        public ActionResult login(UserLogin model)               // post function 
        {

            if (string.IsNullOrEmpty(model.username))
            {
                ModelState.AddModelError("Name", "Please Enter User Name");
            }
            if (string.IsNullOrEmpty(model.password))
            {
                ModelState.AddModelError("MobileNo", "Please enter Password");
            }

            if (ModelState.IsValid)
            {
                DataTable table12 = new DataTable();
                string ErrorMessage = "password or username not found or matched";
                string constr1 = ConfigurationManager.ConnectionStrings["DAP12"].ConnectionString;
                using (SqlConnection con = new SqlConnection(constr1))
                {
                    string query = "select USER_PASSWORD from ss_user where USER_LOGINID=@username and USER_PASSWORD=@password and dept_id=@depertmant";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Connection = con;
                        con.Open();
                        cmd.Parameters.AddWithValue("@username", model.username);
                        // cmd.Parameters.AddWithValue("@password", model.password);
                        cmd.Parameters.AddWithValue("@password", Util.Encrypt(model.password));
                        cmd.Parameters.AddWithValue("@depertmant", model.depertment);
                        SqlDataAdapter ds = new SqlDataAdapter(cmd);
                        ds.Fill(table12);
                        con.Close();
                    }
                }

                //DataTable table = new DataTable();
                //string constr = ConfigurationManager.ConnectionStrings["DAP"].ConnectionString;
                //using (SqlConnection con = new SqlConnection(constr))
                //{
                //    string query = "select * from users where username=@username and password=@password and depertment=@depertmant";
                //    using (SqlCommand cmd = new SqlCommand(query, con))
                //    {
                //        cmd.Connection = con;
                //        con.Open();
                //        cmd.Parameters.AddWithValue("@username",model.username);
                //        cmd.Parameters.AddWithValue("@password", model.password);
                //        cmd.Parameters.AddWithValue("@depertmant", model.depertment);
                //        SqlDataAdapter ds = new SqlDataAdapter(cmd);
                //        ds.Fill(table);
                //        con.Close();
                //    }
                //}                   //  some work needs to be done 
                if (table12.Rows.Count > 0)
                {
                    ErrorMessage = "";
                    model.tcount = "";
                    string my_session = model.username;
                    this.HttpContext.Session["username_session"] = my_session;  // passing value into session
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ViewBag.ErrorMessage = "password or username not found or matched";
                    //return View(model);  
                    model.tcount = "errror";
                    // this.HttpContext.Session["username_session"] = "error";
                    return RedirectToAction("login", "Home", new { emesg = ErrorMessage });
                }    // return to login page or display error page 
            }
            else
            {
                DataTable table = new DataTable();
                string constr = ConfigurationManager.ConnectionStrings["DAP12"].ConnectionString;
                using (SqlConnection con = new SqlConnection(constr))
                {
                    string query = "select DEPT_ID,DEPT_SHORTNAME,DEPT_ISDEFAULT from CONFIG_DEPARTMENT order by DEPT_ISDEFAULT DESC";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        SqlDataAdapter ds = new SqlDataAdapter(cmd);
                        ds.Fill(table);
                    }
                }
                List<UserLogin> d_list = new List<UserLogin>();
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    //  bankname="Anubhav",shortname="Chaudhary",rating="AA+"
                    d_list.Add(new UserLogin()
                    {
                        depertment_id = table.Rows[i]["DEPT_ID"].ToString(),
                        depertment = table.Rows[i]["DEPT_SHORTNAME"].ToString(),
                    });
                }
                UserLogin user1 = new UserLogin();
                user1.dept_list = d_list;
                return View(user1);
                // return RedirectToAction("login", "Home" );
            }
        }
        ////// functions witout validation /////////

        // public ActionResult login(string emesg)               //get 
        //     {

        //        // string encrypted = Util.Encrypt("Today123");

        //      //   string my_session = String.Empty;

        //         UserLogin user1 = new UserLogin();
        //         string my_session=null;
        //         // my_session= this.HttpContext.Session["username_session"].ToString();

        //          if (emesg !=null)
        //          {
        //              user1.tcount ="ERROR";
        //          }

        //         DataTable table = new DataTable();
        //         string constr = ConfigurationManager.ConnectionStrings["DAP12"].ConnectionString;
        //         using (SqlConnection con = new SqlConnection(constr))
        //         {
        //        string query="select DEPT_ID,DEPT_SHORTNAME,DEPT_ISDEFAULT from CONFIG_DEPARTMENT order by DEPT_ISDEFAULT DESC";
        //             using (SqlCommand cmd = new SqlCommand(query, con))
        //             {
        //                 SqlDataAdapter ds = new SqlDataAdapter(cmd);
        //                 ds.Fill(table);
        //             }
        //         }

        //         List<UserLogin> d_list = new List<UserLogin>();

        //         for (int i = 0; i < table.Rows.Count; i++)
        //         {
        //             //  bankname="Anubhav",shortname="Chaudhary",rating="AA+"

        //             d_list.Add(new UserLogin()
        //             {
        //                 depertment_id = table.Rows[i]["DEPT_ID"].ToString(),
        //                 depertment = table.Rows[i]["DEPT_SHORTNAME"].ToString(),

        //             });
        //         }

        //         user1.dept_list = d_list;
        //         //if (this.HttpContext.Session["username_session"] !="")
        //         //{
        //         //    user1.tcount = "ERROR";    
        //         //    return View(user1); 
        //         //}
        //         //else
        //          return View(user1); 

        //}
        //[HttpPost]
        //public ActionResult login(UserLogin model)               // post function 
        //     {
        //         DataTable table12 = new DataTable();

        //         string ErrorMessage = "password or username not found or matched";
        //         string constr1 = ConfigurationManager.ConnectionStrings["DAP12"].ConnectionString;
        //         using (SqlConnection con = new SqlConnection(constr1))
        //         {
        //             string query = "select USER_PASSWORD from ss_user where USER_LOGINID=@username and USER_PASSWORD=@password and dept_id=@depertmant";
        //             using (SqlCommand cmd = new SqlCommand(query, con))
        //             {
        //                 cmd.Connection = con;
        //                 con.Open();
        //                 cmd.Parameters.AddWithValue("@username", model.username);
        //                // cmd.Parameters.AddWithValue("@password", model.password);
        //                 cmd.Parameters.AddWithValue("@password", Util.Encrypt(model.password));
        //                 cmd.Parameters.AddWithValue("@depertmant", model.depertment);
        //                 SqlDataAdapter ds = new SqlDataAdapter(cmd);
        //                 ds.Fill(table12);
        //                 con.Close();
        //             }
        //         }
        //         //DataTable table = new DataTable();
        //         //string constr = ConfigurationManager.ConnectionStrings["DAP"].ConnectionString;
        //         //using (SqlConnection con = new SqlConnection(constr))
        //         //{
        //         //    string query = "select * from users where username=@username and password=@password and depertment=@depertmant";
        //         //    using (SqlCommand cmd = new SqlCommand(query, con))
        //         //    {
        //         //        cmd.Connection = con;
        //         //        con.Open();
        //         //        cmd.Parameters.AddWithValue("@username",model.username);
        //         //        cmd.Parameters.AddWithValue("@password", model.password);
        //         //        cmd.Parameters.AddWithValue("@depertmant", model.depertment);
        //         //        SqlDataAdapter ds = new SqlDataAdapter(cmd);
        //         //        ds.Fill(table);
        //         //        con.Close();
        //         //    }
        //         //}                   //  some work needs to be done 
        //         if (table12.Rows.Count > 0)
        //         {
        //             ErrorMessage = "";
        //             model.tcount = "";
        //             string my_session=model.username;
        //             this.HttpContext.Session["username_session"] =my_session;  // passing value into session
        //             return RedirectToAction("Index", "Home");
        //         }
        //         else
        //         {
        //             ViewBag.ErrorMessage="password or username not found or matched";
        //             //return View(model);  
        //             model.tcount = "errror";
        //            // this.HttpContext.Session["username_session"] = "error";
        //             return RedirectToAction("login", "Home", new { emesg = ErrorMessage });
        //         }    // return to login page or display error page 
        //         }
        public ActionResult Rating()
        {

            if (this.HttpContext.Session["username_session"] != null)
            {

                DataTable table = new DataTable();
                string constr = ConfigurationManager.ConnectionStrings["DAP_NEW"].ConnectionString;
                using (SqlConnection con = new SqlConnection(constr))
                {
                    //string query="select RATING_ID,RATING_NAME,RATING_SHORTNAME,RATING_STATUS,RATING_ISDEFAULT from config_rating ";
                    using (SqlCommand cmd = new SqlCommand("SP_GET_Rating", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        con.Open();
                        SqlDataAdapter ds = new SqlDataAdapter(cmd);
                        ds.Fill(table);
                        con.Close();
                    }
                }
                List<Rating> r_list = new List<Rating>();
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    r_list.Add(new Rating()
                    {
                        rating_id = table.Rows[i]["RATING_ID"].ToString(),
                        rating_name = table.Rows[i]["RATING_NAME"].ToString(),
                        rating_shortname = table.Rows[i]["RATING_SHORTNAME"].ToString(),
                        rating_status = table.Rows[i]["RATING_STATUS"].ToString(),
                        rating_isdefault = table.Rows[i]["RATING_ISDEFAULT"].ToString()
                    });
                }

                Rating user1 = new Rating();
                user1.rating_list = r_list;

                return View(user1);
            }
            else
            {
                return RedirectToAction("login", "Home");

            }	



        }
        public JsonResult edit_rating(Rating model)
        {
            int s = 100;
            string user_session = this.HttpContext.Session["username_session"].ToString();
            string ratingid = model.rating_id.ToString();
            string ratingname = model.rating_name.ToString();
            string ratingshortname = model.rating_shortname.ToString();
            string ratingstatus = model.rating_status.ToString();
            string ratingisdefault = model.rating_isdefault.ToString();
            if (ratingisdefault == "TRUE")
            {
                ratingisdefault = "1";
            }
            if (ratingisdefault == "FALSE")
            {
                ratingisdefault = "0";
            }
            string constr = ConfigurationManager.ConnectionStrings["DAP_NEW"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                con.Open();
                // MessageBox.Show ("Connection Open ! ");
                // cnn.Close();
                SqlCommand cmd = new SqlCommand("SP_UPDATE_RATING", con);    // to be corrected 
                // SqlCommand cmd1 = new SqlCommand("select top 1 * from Customer_IDs where ID_NO ='" + txtCnicNo + "'", oCnn);   
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Rating_Id", model.rating_id);
                cmd.Parameters.AddWithValue("@Rating_Name", model.rating_name);
                cmd.Parameters.AddWithValue("@Rating_ShortName", model.rating_shortname);
                cmd.Parameters.AddWithValue("@Rating_Status", model.rating_status);
                cmd.Parameters.AddWithValue("@Rating_IsDefault", ratingisdefault);
                cmd.Parameters.AddWithValue("@sMUser", user_session);
                s = cmd.ExecuteNonQuery();
                con.Close();
            }
            string message1 = "Successful";
            string message2 = "unsuccessful";
            if (s == 0)
            {
                //TempData["Success"] = "Un Successfull";
                //return RedirectToAction("Rating", "Home");                
                return Json(message2, JsonRequestBehavior.AllowGet);
            }
            else
            {
                //TempData["Success"] = "Edit Successfully!";
                //return RedirectToAction("Rating", "Home");
                return Json(message1, JsonRequestBehavior.AllowGet);
            }
            //return View();
        }
        [HttpPost]
        public ActionResult Rating(Rating model)
        {
            if (string.IsNullOrEmpty(model.rating_name))
            {
                ModelState.AddModelError("Name", "Kindly Provide Name");
            }
            if (string.IsNullOrEmpty(model.rating_shortname))
            {
                ModelState.AddModelError("Name", "Kindly Provide Name");
            }
            if (string.IsNullOrEmpty(model.rating_status))
            {
                ModelState.AddModelError("Name", "Kindly Provide Name");
            }

            if (ModelState.IsValid)
            {
                int s = 100;
                string user_session = this.HttpContext.Session["username_session"].ToString();
                // string ratingid = model.rating_id.ToString();
                string ratingname = model.rating_name.ToString();
                string ratingshortname = model.rating_shortname.ToString();
                string ratingstatus = model.rating_status.ToString();
                //string ratingisdefault = model.rating_isdefault.ToString();
                bool rating_default = model.rating_isdefault_bool;

                string constr = ConfigurationManager.ConnectionStrings["DAP_NEW"].ConnectionString;
                using (SqlConnection con = new SqlConnection(constr))
                {

                    con.Open();
                    // MessageBox.Show ("Connection Open ! ");
                    // cnn.Close();
                    SqlCommand cmd = new SqlCommand("SP_INSERT_RATING", con);    // to be corrected 
                    // SqlCommand cmd1 = new SqlCommand("select top 1 * from Customer_IDs where ID_NO ='" + txtCnicNo + "'", oCnn);   
                    cmd.CommandType = CommandType.StoredProcedure;
                    //cmd.Parameters.AddWithValue("@Rating_Id", model.rating_id);
                    cmd.Parameters.AddWithValue("@Rating_Name", model.rating_name);
                    cmd.Parameters.AddWithValue("@Rating_ShortName", model.rating_shortname);
                    cmd.Parameters.AddWithValue("@Rating_Status", model.rating_status);
                    cmd.Parameters.AddWithValue("@Rating_IsDefault", model.rating_isdefault_bool);
                    cmd.Parameters.AddWithValue("@sUser", user_session);
                    s = cmd.ExecuteNonQuery();
                    con.Close();
                }

                return RedirectToAction("Rating", "Home");
            }
            else
            {
                DataTable table = new DataTable();
                string constr = ConfigurationManager.ConnectionStrings["DAP_NEW"].ConnectionString;
                using (SqlConnection con = new SqlConnection(constr))
                {
                    //string query="select RATING_ID,RATING_NAME,RATING_SHORTNAME,RATING_STATUS,RATING_ISDEFAULT from config_rating ";
                    using (SqlCommand cmd = new SqlCommand("SP_GET_Rating", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        con.Open();
                        SqlDataAdapter ds = new SqlDataAdapter(cmd);
                        ds.Fill(table);
                        con.Close();
                    }
                }

                List<Rating> r_list = new List<Rating>();
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    r_list.Add(new Rating()
                    {
                        rating_id = table.Rows[i]["RATING_ID"].ToString(),
                        rating_name = table.Rows[i]["RATING_NAME"].ToString(),
                        rating_shortname = table.Rows[i]["RATING_SHORTNAME"].ToString(),
                        rating_status = table.Rows[i]["RATING_STATUS"].ToString(),
                        rating_isdefault = table.Rows[i]["RATING_ISDEFAULT"].ToString()
                    });
                }
                Rating user1 = new Rating();
                user1.rating_list = r_list;
                return View(user1);
                //return View();            
            }

        }
        [HttpPost]
        public ActionResult ExportData_rating()
     {

         string txtFile = string.Empty;        
      DataTable table = new DataTable();
        string constr = ConfigurationManager.ConnectionStrings["DAP_NEW"].ConnectionString;
        using (SqlConnection con = new SqlConnection(constr))
         {
                //string query="select RATING_ID,RATING_NAME,RATING_SHORTNAME,RATING_STATUS,RATING_ISDEFAULT from config_rating ";
                using (SqlCommand cmd = new SqlCommand("SP_GET_Rating", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    con.Open();
                    SqlDataAdapter ds = new SqlDataAdapter(cmd);
                    ds.Fill(table);
                    con.Close();
                 }
        }
        List<Rating_pdf> r_list = new List<Rating_pdf>();
        for (int i = 0; i < table.Rows.Count; i++)
        {
            r_list.Add(new Rating_pdf()
            {
                Serial = table.Rows[i]["RATING_ID"].ToString(),
                Name = table.Rows[i]["RATING_NAME"].ToString(),
                Shortname = table.Rows[i]["RATING_SHORTNAME"].ToString(),
                Status = table.Rows[i]["RATING_STATUS"].ToString(),
                IsDefault = table.Rows[i]["RATING_ISDEFAULT"].ToString(),
                CreatedBy = table.Rows[i]["RATING_CREATEDBY"].ToString(),
                CreatedOn = table.Rows[i]["RATING_CREATEDON"].ToString(),
                ModifiedBy = table.Rows[i]["RATING_MODIFIEDBY"].ToString(),
                ModifiedOn = table.Rows[i]["RATING_MODIFIEDON"].ToString(),                       
            });
        }
        GridView gv = new GridView();
        Response.ContentType = "application/pdf";
        Response.AddHeader("content-disposition", "attachment;filename=Rating_Report.pdf");
        Response.Cache.SetCacheability(HttpCacheability.NoCache);
        StringWriter sw = new StringWriter();
        HtmlTextWriter hw = new HtmlTextWriter(sw);
        gv.DataSource = r_list;  
        gv.AllowPaging = false;     
        gv.DataBind();
      //for (int i = 0; i < gv.Columns.Count; i++)
      //{
      //    //   gv.Columns[0].HeaderText = "Serial";
      //    gv.Columns[i].ItemStyle.Width = 400;
      //    gv.Columns[i].ItemStyle.Wrap = false;
      //}    
       gv.HeaderRow.Style.Add("width", "300%");
       gv.HeaderRow.Style.Add("font-size", "7px");
       gv.Style.Add("text-decoration", "none");
       gv.Style.Add("font-family", "Arial, Helvetica, sans-serif;");
       gv.Style.Add("font-size", "7px");
        //gv.Style.Add("width", "300%");         
        gv.RenderControl(hw);
        StringReader sr = new StringReader(sw.ToString());
        Document pdfDoc = new Document(PageSize.A4.Rotate(), 30f, 30f, 30f, 30f);
        HTMLWorker htmlparser = new HTMLWorker(pdfDoc);
        PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
        pdfDoc.Open();
        htmlparser.Parse(sr);//this is the error line
        pdfDoc.Close();
        Response.Write(pdfDoc);
        Response.End();  
       return RedirectToAction("Rating","Home");         
                
     }
        public ActionResult bank_type()
        {
            if (this.HttpContext.Session["username_session"] != null)
            {


                DataTable table = new DataTable();
                string constr = ConfigurationManager.ConnectionStrings["DAP_NEW"].ConnectionString;
                using (SqlConnection con = new SqlConnection(constr))
                {
                    //string query="select RATING_ID,RATING_NAME,RATING_SHORTNAME,RATING_STATUS,RATING_ISDEFAULT from config_rating ";
                    using (SqlCommand cmd = new SqlCommand("SP_GET_CONFIG_BANKTYPE", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        con.Open();
                        SqlDataAdapter ds = new SqlDataAdapter(cmd);
                        ds.Fill(table);
                        con.Close();
                    }
                }

                List<Bank_Type> bt_list = new List<Bank_Type>();

                for (int i = 0; i < table.Rows.Count; i++)
                {
                    //  bankname="Anubhav",shortname="Chaudhary",rating="AA+"

                    bt_list.Add(new Bank_Type()
                    {
                        BNKTYPE_ID = table.Rows[i]["BNKTYPE_ID"].ToString(),
                        BNKTYPE_NAME = table.Rows[i]["BNKTYPE_NAME"].ToString(),
                        BNKTYPE_SHORTNAME = table.Rows[i]["BNKTYPE_SHORTNAME"].ToString(),
                        BNKTYPE_STATUS = table.Rows[i]["BNKTYPE_STATUS"].ToString(),
                        BNKTYPE_ISDEFAULT = table.Rows[i]["BNKTYPE_ISDEFAULT"].ToString()

                    });
                }

                Bank_Type user1 = new Bank_Type();
                user1.bank_type_list = bt_list;

                return View(user1);

            }
            else
            {
                return RedirectToAction("login", "Home");
            }
             
             
             }
        public JsonResult edit_bank_type(Bank_Type model)
        {
            int s = 100;
            string user_session = this.HttpContext.Session["username_session"].ToString();
            string banktypeid = model.BNKTYPE_ID.ToString();
            string banktypename = model.BNKTYPE_NAME.ToString();
            string banktypeshortname = model.BNKTYPE_SHORTNAME.ToString();
            string banktypestatus = model.BNKTYPE_STATUS.ToString();
            string ratingisdefault = model.BNKTYPE_ISDEFAULT.ToString();
            if (ratingisdefault == "TRUE")
            {
                ratingisdefault = "1";
            }
            if (ratingisdefault == "FALSE" || ratingisdefault == "False")
            {
                ratingisdefault = "0";
            }

            string constr = ConfigurationManager.ConnectionStrings["DAP_NEW"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {

                con.Open();
                // MessageBox.Show ("Connection Open ! ");
                // cnn.Close();
                SqlCommand cmd = new SqlCommand("SP_UPDATE_CONFIG_BANKTYPE", con);    // to be corrected 
                // SqlCommand cmd1 = new SqlCommand("select top 1 * from Customer_IDs where ID_NO ='" + txtCnicNo + "'", oCnn);   
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@BNKTYPE_Id", Int32.Parse(model.BNKTYPE_ID));
                cmd.Parameters.AddWithValue("@BNKTYPE_Name", model.BNKTYPE_NAME);
                cmd.Parameters.AddWithValue("@BNKTYPE_ShortName", model.BNKTYPE_SHORTNAME);
                cmd.Parameters.AddWithValue("@BNKTYPE_Status", model.BNKTYPE_STATUS);
                cmd.Parameters.AddWithValue("@BNKTYPE_IsDefault", ratingisdefault);
                cmd.Parameters.AddWithValue("@sUser", user_session);
                s = cmd.ExecuteNonQuery();
                con.Close();
            }
            string message1 = "Successful";
            string message2 = "unsuccessful";
            if (s == 0)
            {
                return Json(message2, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(message1, JsonRequestBehavior.AllowGet);
            }

            //return View();
        }
        [HttpPost]
        public ActionResult bank_type(Bank_Type model)
        {
            if (string.IsNullOrEmpty(model.BNKTYPE_NAME))
            {
                ModelState.AddModelError("Name", "Kindly Provide Name");
            }
            if (string.IsNullOrEmpty(model.BNKTYPE_SHORTNAME))
            {
                ModelState.AddModelError("Name", "Kindly Provide Name");
            }
            if (string.IsNullOrEmpty(model.BNKTYPE_STATUS))
            {
                ModelState.AddModelError("Name", "Kindly Provide Name");
            }

            if (ModelState.IsValid)
            {
                int s = 100;
                string user_session = this.HttpContext.Session["username_session"].ToString();
                // string ratingid = model.rating_id.ToString();
                string banktypename = model.BNKTYPE_NAME.ToString();
                string banktypeshortname = model.BNKTYPE_SHORTNAME.ToString();
                string banktypestatus = model.BNKTYPE_STATUS.ToString();
                //string ratingisdefault = model.rating_isdefault.ToString();
                bool banktypedefault = model.BNKTYPE_ISDEFAULT_BOOL;
                string constr = ConfigurationManager.ConnectionStrings["DAP_NEW"].ConnectionString;
                using (SqlConnection con = new SqlConnection(constr))
                {
                    con.Open();
                    // cnn.Close();
                    SqlCommand cmd = new SqlCommand("SP_INSERT_CONFIG_BANKTYPE", con);    // to be corrected 
                    // SqlCommand cmd1 = new SqlCommand("select top 1 * from Customer_IDs where ID_NO ='" + txtCnicNo + "'", oCnn);   
                    cmd.CommandType = CommandType.StoredProcedure;
                    //cmd.Parameters.AddWithValue("@Rating_Id", model.rating_id);
                    cmd.Parameters.AddWithValue("@BNKTYPE_Name", model.BNKTYPE_NAME);
                    cmd.Parameters.AddWithValue("@BNKTYPE_ShortName", model.BNKTYPE_SHORTNAME);
                    cmd.Parameters.AddWithValue("@BNKTYPE_Status", model.BNKTYPE_STATUS);
                    cmd.Parameters.AddWithValue("@BNKTYPE_IsDefault", model.BNKTYPE_ISDEFAULT_BOOL);
                    cmd.Parameters.AddWithValue("@sUser", user_session);
                    s = cmd.ExecuteNonQuery();
                    con.Close();
                }
                return RedirectToAction("bank_type", "Home");
            }
            else
            {
                DataTable table = new DataTable();
                string constr = ConfigurationManager.ConnectionStrings["DAP_NEW"].ConnectionString;
                using (SqlConnection con = new SqlConnection(constr))
                {
                    //string query="select RATING_ID,RATING_NAME,RATING_SHORTNAME,RATING_STATUS,RATING_ISDEFAULT from config_rating ";
                    using (SqlCommand cmd = new SqlCommand("SP_GET_CONFIG_BANKTYPE", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        con.Open();
                        SqlDataAdapter ds = new SqlDataAdapter(cmd);
                        ds.Fill(table);
                        con.Close();
                    }
                }
                List<Bank_Type> bt_list = new List<Bank_Type>();
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    bt_list.Add(new Bank_Type()
                    {
                        BNKTYPE_ID = table.Rows[i]["BNKTYPE_ID"].ToString(),
                        BNKTYPE_NAME = table.Rows[i]["BNKTYPE_NAME"].ToString(),
                        BNKTYPE_SHORTNAME = table.Rows[i]["BNKTYPE_SHORTNAME"].ToString(),
                        BNKTYPE_STATUS = table.Rows[i]["BNKTYPE_STATUS"].ToString(),
                        BNKTYPE_ISDEFAULT = table.Rows[i]["BNKTYPE_ISDEFAULT"].ToString()

                    });
                }
                Bank_Type user1 = new Bank_Type();
                user1.bank_type_list = bt_list;
                return View(user1);
            }


        }
        [HttpPost]
        public ActionResult ExportData_bank_type()
        {

            string txtFile = string.Empty;
            DataTable table = new DataTable();
            string constr = ConfigurationManager.ConnectionStrings["DAP_NEW"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                //string query="select RATING_ID,RATING_NAME,RATING_SHORTNAME,RATING_STATUS,RATING_ISDEFAULT from config_rating ";
                using (SqlCommand cmd = new SqlCommand("SP_GET_CONFIG_BANKTYPE", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    con.Open();
                    SqlDataAdapter ds = new SqlDataAdapter(cmd);
                    ds.Fill(table);
                    con.Close();
                }
            }
            List<Bank_Type_pdf> act_list = new List<Bank_Type_pdf>();
            for (int i = 0; i < table.Rows.Count; i++)
            {
                act_list.Add(new Bank_Type_pdf()
                {
                    Serial = table.Rows[i]["BNKTYPE_ID"].ToString(),
                    Name = table.Rows[i]["BNKTYPE_NAME"].ToString(),
                    Shortname = table.Rows[i]["BNKTYPE_SHORTNAME"].ToString(),
                    Status = table.Rows[i]["BNKTYPE_STATUS"].ToString(),
                    IsDefault = table.Rows[i]["BNKTYPE_ISDEFAULT"].ToString(),
                    CreatedBy = table.Rows[i]["BNKTYPE_CREATEDBY"].ToString(),
                    CreatedOn = table.Rows[i]["BNKTYPE_CREATEDON"].ToString(),
                    ModifiedBy = table.Rows[i]["BNKTYPE_MODIFIEDBY"].ToString(),
                    ModifiedOn = table.Rows[i]["BNKTYPE_MODIFIEDON"].ToString(),
                });
            }
            GridView gv = new GridView();
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "attachment;filename=Bank_Type_Report.pdf");
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            StringWriter sw = new StringWriter();
            HtmlTextWriter hw = new HtmlTextWriter(sw);
            gv.DataSource = act_list;
            gv.AllowPaging = false;
            gv.DataBind();
            //for (int i = 0; i < gv.Columns.Count; i++)
            //{
            //    //   gv.Columns[0].HeaderText = "Serial";
            //    gv.Columns[i].ItemStyle.Width = 400;
            //    gv.Columns[i].ItemStyle.Wrap = false;
            //}    
            gv.HeaderRow.Style.Add("width", "300%");
            gv.HeaderRow.Style.Add("font-size", "7px");
            gv.Style.Add("text-decoration", "none");
            gv.Style.Add("font-family", "Arial, Helvetica, sans-serif;");
            gv.Style.Add("font-size", "7px");
            //gv.Style.Add("width", "300%");         
            gv.RenderControl(hw);
            StringReader sr = new StringReader(sw.ToString());
            Document pdfDoc = new Document(PageSize.A4.Rotate(), 30f, 30f, 30f, 30f);
            HTMLWorker htmlparser = new HTMLWorker(pdfDoc);
            PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
            pdfDoc.Open();
            htmlparser.Parse(sr);//this is the error line
            pdfDoc.Close();
            Response.Write(pdfDoc);
            Response.End();
            return RedirectToAction("bank_type", "Home");

        }        
        public ActionResult account_type()
        {
             if (this.HttpContext.Session["username_session"] != null)
    {
            
            
            DataTable table = new DataTable();
            string constr = ConfigurationManager.ConnectionStrings["DAP_NEW"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                //string query="select RATING_ID,RATING_NAME,RATING_SHORTNAME,RATING_STATUS,RATING_ISDEFAULT from config_rating ";
                using (SqlCommand cmd = new SqlCommand("SP_GET_CONFIG_ACCOUNTTYPE", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    con.Open();
                    SqlDataAdapter ds = new SqlDataAdapter(cmd);
                    ds.Fill(table);
                    con.Close();
                }
            }

            List<Account_Type> act_list = new List<Account_Type>();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                //  bankname="Anubhav",shortname="Chaudhary",rating="AA+"

                act_list.Add(new Account_Type()
                {
                    ACCTYPE_ID = table.Rows[i]["ACCTYPE_ID"].ToString(),
                    ACCTYPE_NAME = table.Rows[i]["ACCTYPE_NAME"].ToString(),
                    ACCTYPE_SHORTNAME = table.Rows[i]["ACCTYPE_SHORTNAME"].ToString(),
                    ACCTYPE_STATUS = table.Rows[i]["ACCTYPE_STATUS"].ToString(),
                    ACCTYPE_ISDEFAULT = table.Rows[i]["ACCTYPE_ISDEFAULT"].ToString()

                });
            }

            Account_Type user1 = new Account_Type();
            user1.account_type_list = act_list;
            return View(user1);
    }
             else
             {
                 return RedirectToAction("login", "Home");

             }	
             
             
             }
        public JsonResult edit_account_type(Account_Type model)
        {
            int s = 100;
            string user_session = this.HttpContext.Session["username_session"].ToString();
            string accounttypeid = model.ACCTYPE_ID.ToString();
            string accounttypename = model.ACCTYPE_NAME.ToString();
            string accountshortname = model.ACCTYPE_SHORTNAME.ToString();
            string accounttypestatus = model.ACCTYPE_STATUS.ToString();
            string ratingisdefault = model.ACCTYPE_ISDEFAULT.ToString();
            if(ratingisdefault == "TRUE")
            {
                ratingisdefault = "1";
            }
            if(ratingisdefault == "FALSE")
            {
                ratingisdefault = "0";
            }
            string constr = ConfigurationManager.ConnectionStrings["DAP_NEW"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {

                con.Open();
                // MessageBox.Show ("Connection Open ! ");
                // cnn.Close();
                SqlCommand cmd = new SqlCommand("SP_UPDATE_CONFIG_ACCOUNTTYPE", con);    // to be corrected 
                // SqlCommand cmd1 = new SqlCommand("select top 1 * from Customer_IDs where ID_NO ='" + txtCnicNo + "'", oCnn);   
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ACCTYPE_Id", model.ACCTYPE_ID);
                cmd.Parameters.AddWithValue("@ACCTYPE_Name", model.ACCTYPE_NAME);
                cmd.Parameters.AddWithValue("@ACCTYPE_ShortName", model.ACCTYPE_SHORTNAME);
                cmd.Parameters.AddWithValue("@ACCTYPE_Status", model.ACCTYPE_STATUS);
                cmd.Parameters.AddWithValue("@ACCTYPE_IsDefault", ratingisdefault);
                cmd.Parameters.AddWithValue("@sUser", user_session);
                s = cmd.ExecuteNonQuery();
                con.Close();
            }
            string message1 = "Successful";
            string message2 = "unsuccessful";
            if (s == 0)
            {
                return Json(message2, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(message1, JsonRequestBehavior.AllowGet);
            }
            //return View();
        }
        [HttpPost]
        public ActionResult account_type(Account_Type model)
        {

            if (string.IsNullOrEmpty(model.ACCTYPE_NAME))
            {
                ModelState.AddModelError("Name", "Kindly Provide Name");
            }
            if (string.IsNullOrEmpty(model.ACCTYPE_SHORTNAME))
            {
                ModelState.AddModelError("Name", "Kindly Provide Name");
            }
            if (string.IsNullOrEmpty(model.ACCTYPE_STATUS))
            {
                ModelState.AddModelError("Name", "Kindly Provide Name");
            }
            if (ModelState.IsValid)
            {

                int s = 100;
                string user_session = this.HttpContext.Session["username_session"].ToString();
                // string ratingid = model.rating_id.ToString();
                string accttypename = model.ACCTYPE_NAME.ToString();
                string accttypeshortname = model.ACCTYPE_SHORTNAME.ToString();
                string accttypestatus = model.ACCTYPE_STATUS.ToString();
                //string ratingisdefault = model.rating_isdefault.ToString();
                bool accttypedefault = model.ACCTYPE_ISDEFAULT_BOOL;

                string constr = ConfigurationManager.ConnectionStrings["DAP_NEW"].ConnectionString;
                using (SqlConnection con = new SqlConnection(constr))
                {

                    con.Open();
                    // MessageBox.Show ("Connection Open ! ");
                    // cnn.Close();
                    SqlCommand cmd = new SqlCommand("SP_INSERT_CONFIG_ACCOUNTTYPE", con);    // to be corrected 
                    // SqlCommand cmd1 = new SqlCommand("select top 1 * from Customer_IDs where ID_NO ='" + txtCnicNo + "'", oCnn);   
                    cmd.CommandType = CommandType.StoredProcedure;
                    //cmd.Parameters.AddWithValue("@Rating_Id", model.rating_id);
                    cmd.Parameters.AddWithValue("@ACCTYPE_Name", model.ACCTYPE_NAME);
                    cmd.Parameters.AddWithValue("@ACCTYPE_ShortName", model.ACCTYPE_SHORTNAME);
                    cmd.Parameters.AddWithValue("@ACCTYPE_Status", model.ACCTYPE_STATUS);
                    cmd.Parameters.AddWithValue("@ACCTYPE_IsDefault", model.ACCTYPE_ISDEFAULT_BOOL);
                    cmd.Parameters.AddWithValue("@sUser", user_session);
                    s = cmd.ExecuteNonQuery();
                    con.Close();
                }
                return RedirectToAction("account_type", "Home");
            }
            else
            {
                DataTable table = new DataTable();
                string constr = ConfigurationManager.ConnectionStrings["DAP_NEW"].ConnectionString;
                using (SqlConnection con = new SqlConnection(constr))
                {
                    //string query="select RATING_ID,RATING_NAME,RATING_SHORTNAME,RATING_STATUS,RATING_ISDEFAULT from config_rating ";
                    using (SqlCommand cmd = new SqlCommand("SP_GET_CONFIG_ACCOUNTTYPE", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        con.Open();
                        SqlDataAdapter ds = new SqlDataAdapter(cmd);
                        ds.Fill(table);
                        con.Close();
                    }
                }
                List<Account_Type> act_list = new List<Account_Type>();
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    //  bankname="Anubhav",shortname="Chaudhary",rating="AA+"
                    act_list.Add(new Account_Type()
                    {
                        ACCTYPE_ID = table.Rows[i]["ACCTYPE_ID"].ToString(),
                        ACCTYPE_NAME = table.Rows[i]["ACCTYPE_NAME"].ToString(),
                        ACCTYPE_SHORTNAME = table.Rows[i]["ACCTYPE_SHORTNAME"].ToString(),
                        ACCTYPE_STATUS = table.Rows[i]["ACCTYPE_STATUS"].ToString(),
                        ACCTYPE_ISDEFAULT = table.Rows[i]["ACCTYPE_ISDEFAULT"].ToString()

                    });
                }
                Account_Type user1 = new Account_Type();
                user1.account_type_list = act_list;
                return View(user1);
            }

        }
        [HttpPost]
        public ActionResult ExportData_account_type()
        {

            string txtFile = string.Empty;
            DataTable table = new DataTable();
            string constr = ConfigurationManager.ConnectionStrings["DAP_NEW"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                //string query="select RATING_ID,RATING_NAME,RATING_SHORTNAME,RATING_STATUS,RATING_ISDEFAULT from config_rating ";
                using (SqlCommand cmd = new SqlCommand("SP_GET_CONFIG_ACCOUNTTYPE", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    con.Open();
                    SqlDataAdapter ds = new SqlDataAdapter(cmd);
                    ds.Fill(table);
                    con.Close();
                }
            }
            List<Account_Type_pdf> act_list = new List<Account_Type_pdf>();
            for (int i = 0; i < table.Rows.Count; i++)
            {
                act_list.Add(new Account_Type_pdf()
                {
                    Serial = table.Rows[i]["ACCTYPE_ID"].ToString(),
                    Name = table.Rows[i]["ACCTYPE_NAME"].ToString(),
                    Shortname = table.Rows[i]["ACCTYPE_SHORTNAME"].ToString(),
                    Status = table.Rows[i]["ACCTYPE_STATUS"].ToString(),
                    IsDefault = table.Rows[i]["ACCTYPE_ISDEFAULT"].ToString(),
                    CreatedBy = table.Rows[i]["ACCTYPE_CREATEDBY"].ToString(),
                    CreatedOn = table.Rows[i]["ACCTYPE_CREATEDON"].ToString(),
                    ModifiedBy = table.Rows[i]["ACCTYPE_MODIFIEDBY"].ToString(),
                    ModifiedOn = table.Rows[i]["ACCTYPE_MODIFIEDON"].ToString(),
                });
            }
            GridView gv = new GridView();
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "attachment;filename=Account_Type_Report.pdf");
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            StringWriter sw = new StringWriter();
            HtmlTextWriter hw = new HtmlTextWriter(sw);
            gv.DataSource = act_list;
            gv.AllowPaging = false;
            gv.DataBind();
            //for (int i = 0; i < gv.Columns.Count; i++)
            //{
            //    //   gv.Columns[0].HeaderText = "Serial";
            //    gv.Columns[i].ItemStyle.Width = 400;
            //    gv.Columns[i].ItemStyle.Wrap = false;
            //}    
            gv.HeaderRow.Style.Add("width", "300%");
            gv.HeaderRow.Style.Add("font-size", "7px");
            gv.Style.Add("text-decoration", "none");
            gv.Style.Add("font-family", "Arial, Helvetica, sans-serif;");
            gv.Style.Add("font-size", "7px");
            //gv.Style.Add("width", "300%");         
            gv.RenderControl(hw);
            StringReader sr = new StringReader(sw.ToString());
            Document pdfDoc = new Document(PageSize.A4.Rotate(), 30f, 30f, 30f, 30f);
            HTMLWorker htmlparser = new HTMLWorker(pdfDoc);
            PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
            pdfDoc.Open();
            htmlparser.Parse(sr);//this is the error line
            pdfDoc.Close();
            Response.Write(pdfDoc);
            Response.End();
            return RedirectToAction("account_type", "Home");

        }              
        public ActionResult bank_setup()
        {
            if (this.HttpContext.Session["username_session"] != null)
    {
            ////////////////////////////////#####################################//////////////////////
            string constr = ConfigurationManager.ConnectionStrings["DAP_NEW"].ConnectionString;
            DataTable table = new DataTable();
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = "SELECT RATING_ID,RATING_NAME FROM CONFIG_RATING";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    SqlDataAdapter ds = new SqlDataAdapter(cmd);
                    ds.Fill(table);
                }
            }
            List<Bank_Setup> r_list = new List<Bank_Setup>();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                //  bankname="Anubhav",shortname="Chaudhary",rating="AA+"

                r_list.Add(new Bank_Setup()
                {
                    RATING_ID = table.Rows[i]["RATING_ID"].ToString(),
                    RATING_NAME = table.Rows[i]["RATING_NAME"].ToString(),
                });
            }
            //////////////////////////////#######################################################////////////////////////

            DataTable table1 = new DataTable();
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = "SELECT BNKTYPE_ID,BNKTYPE_NAME FROM CONFIG_BANKTYPE order by BNKTYPE_ISDEFAULT desc  ";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    SqlDataAdapter ds = new SqlDataAdapter(cmd);
                    ds.Fill(table1);
                }
            }
            List<Bank_Setup> banktype_list = new List<Bank_Setup>();
            for (int i = 0; i < table1.Rows.Count; i++)
            {
                //  bankname="Anubhav",shortname="Chaudhary",rating="AA+"
                banktype_list.Add(new Bank_Setup()
                {
                    BNKTYPE_ID = table1.Rows[i]["BNKTYPE_ID"].ToString(),
                    BNKTYPE_NAME = table1.Rows[i]["BNKTYPE_NAME"].ToString(),
                });
            }

            //////////////////////////////#######################################################////////////////////////
            DataTable table2 = new DataTable();
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = "select CITY_ID,CITY_NAME from REGION_CITY";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    SqlDataAdapter ds = new SqlDataAdapter(cmd);
                    ds.Fill(table2);
                }
            }
            List<Bank_Setup> city_list = new List<Bank_Setup>();
            for (int i = 0; i < table2.Rows.Count; i++)
            {
                //  bankname="Anubhav",shortname="Chaudhary",rating="AA+"
                city_list.Add(new Bank_Setup()
                {
                    CITY_ID = table2.Rows[i]["CITY_ID"].ToString(),
                    CITY_NAME = table2.Rows[i]["CITY_NAME"].ToString(),
                });
            }
            //////////////////////////////#######################################################////////////////////////
            DataTable table3 = new DataTable();
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = "select STATE_ID,STATE_NAME from REGION_STATE";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    SqlDataAdapter ds = new SqlDataAdapter(cmd);
                    ds.Fill(table3);
                }
            }
            List<Bank_Setup> state_list = new List<Bank_Setup>();

            for (int i = 0; i < table3.Rows.Count; i++)
            {
                //  bankname="Anubhav",shortname="Chaudhary",rating="AA+"
                state_list.Add(new Bank_Setup()
                {
                    STATE_ID = table3.Rows[i]["STATE_ID"].ToString(),
                    STATE_NAME = table3.Rows[i]["STATE_NAME"].ToString(),
                });
            }
            //////////////////////////////#######################################################////////////////////////
            DataTable table4 = new DataTable();
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = "select COUNTRY_ID,COUNTRY_OFFICIALNAME from REGION_COUNTRY";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    SqlDataAdapter ds = new SqlDataAdapter(cmd);
                    ds.Fill(table4);
                }
            }
            List<Bank_Setup> country_list = new List<Bank_Setup>();
            for (int i = 0; i < table4.Rows.Count; i++)
            {
                //  bankname="Anubhav",shortname="Chaudhary",rating="AA+"
                country_list.Add(new Bank_Setup()
                {
                    COUNTRY_ID = table4.Rows[i]["COUNTRY_ID"].ToString(),
                    COUNTRY_NAME = table4.Rows[i]["COUNTRY_OFFICIALNAME"].ToString(),
                });
            }

            DataTable table5 = new DataTable();
            //string constr = ConfigurationManager.ConnectionStrings["DAP_NEW"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                //string query="select RATING_ID,RATING_NAME,RATING_SHORTNAME,RATING_STATUS,RATING_ISDEFAULT from config_rating ";
                using (SqlCommand cmd = new SqlCommand("SP_GET_STP_BANK", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    con.Open();
                    SqlDataAdapter ds = new SqlDataAdapter(cmd);
                    ds.Fill(table5);
                    con.Close();
                }
            }

            List<Bank_Setup> bank_setup_list = new List<Bank_Setup>();
            for (int i = 0; i < table5.Rows.Count; i++)
            {                
                bank_setup_list.Add(new Bank_Setup()
                {
                    BANK_ID = table5.Rows[i]["Bank_Id"].ToString(),
                    Bank_Name = table5.Rows[i]["Bank_Name"].ToString(),
                    Bank_ShortName = table5.Rows[i]["Bank_ShortName"].ToString(),
                    Bank_Code = table5.Rows[i]["Bank_Code"].ToString(),                    
                    BNKTYPE_ID = table5.Rows[i]["BNKTYPE_ID"].ToString(),                    
                    BNKTYPE_NAME = table5.Rows[i]["BNKTYPE_NAME"].ToString(),
                    
                    RATING_ID = table5.Rows[i]["RATING_ID"].ToString(),
                    RATING_NAME = table5.Rows[i]["RATING_NAME"].ToString(),
                    
                    BANK_STATUS = table5.Rows[i]["BANK_STATUS"].ToString(),
                    BANK_ADDRESSLINE1 = table5.Rows[i]["BANK_ADDRESSLINE1"].ToString(),
                    BANK_ADDRESSLINE2 = table5.Rows[i]["BANK_ADDRESSLINE2"].ToString(),

                    COUNTRY_ID = table5.Rows[i]["COUNTRY_ID"].ToString(),
                    COUNTRY_NAME = table5.Rows[i]["COUNTRY_OFFICIALNAME"].ToString(),
                    STATE_NAME = table5.Rows[i]["STATE_NAME"].ToString(),
                    CITY_ID = table5.Rows[i]["CITY_ID"].ToString(),
                    CITY_NAME = table5.Rows[i]["CITY_NAME"].ToString(),
                    BANK_PHONES = table5.Rows[i]["BANK_PHONES"].ToString(),
                    BANK_FAX = table5.Rows[i]["BANK_FAX"].ToString(),
                    BANK_EMAIL = table5.Rows[i]["BANK_EMAIL"].ToString(),
                    BANK_WEBSITE = table5.Rows[i]["BANK_WEBSITE"].ToString(),
                    BANK_CONTACTPERSONNAME = table5.Rows[i]["BANK_CONTACTPERSONNAME"].ToString(),
                    BANK_CONTACTPERSONMOBILE = table5.Rows[i]["BANK_CONTACTPERSONMOBILE"].ToString(),
                    BANK_CONTACTPERSONEMAIL = table5.Rows[i]["BANK_CONTACTPERSONEMAIL"].ToString(),
                    BANK_isdefault = table5.Rows[i]["ISDEFAULT"].ToString()



                });
            }
            Bank_Setup user1 = new Bank_Setup();
            user1.rating_list = r_list;
            user1.city_list = city_list;
            user1.state_list = state_list;
            user1.bank_type_list = banktype_list;
            user1.country_list = country_list;
            user1.bank_setup_list = bank_setup_list;
            return View(user1);
    }
            else
            {
                return RedirectToAction("login", "Home");
            }

        }
        [HttpPost]
        public ActionResult bank_setup(Bank_Setup model)
        {
            if (string.IsNullOrEmpty(model.Bank_Name))
            {
                ModelState.AddModelError("Name", "Please Enter Bank Name");
            }
            //if (string.IsNullOrEmpty(model.BANK_ADDRESSLINE1))
            //{
            //    ModelState.AddModelError("Name", "Please Enter Bank Name");
            //}
            if (string.IsNullOrEmpty(model.Bank_ShortName))
            {
                ModelState.AddModelError("Name", "Please Enter short  Bank Name");
            }

            //if (!string.IsNullOrEmpty(model.BANK_EMAIL))
            //{
            //    string emailRegex = @"^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$";
            //    Regex re = new Regex(emailRegex);
            //    if (!re.IsMatch(model.BANK_EMAIL))
            //    {
            //        ModelState.AddModelError("Email", "Please Enter Correct Email Address");
            //    }
            //}
            //else
            //{
            //    ModelState.AddModelError("Email", "Please Enter Email Address");
            //}

            model.BANK_EMAIL = "";




           //if (!string.IsNullOrEmpty(model.Bank_Code))
           // {

           //     int a = model.Bank_Code.Length;
           //     if ((a > 4)||(a<3))
           //     { ModelState.AddModelError("Name", "Please Enter the correct bank code"); }

           // }
           // else
           // {
           //     ModelState.AddModelError("Name", "Please Enter Bank Code");
           // }

            model.Bank_Code = "";
            
            if (string.IsNullOrEmpty(model.RATING_ID))
            {
                ModelState.AddModelError("Name", "Please Enter Bank Rating");
            }
            if (string.IsNullOrEmpty(model.CITY_ID))
            {
                ModelState.AddModelError("Name", "Please Enter Bank Rating");
            }
            if (string.IsNullOrEmpty(model.COUNTRY_ID))
            {
                ModelState.AddModelError("Name", "Please Enter Bank Rating");
            }
            if (string.IsNullOrEmpty(model.STATE_ID))
            {
                ModelState.AddModelError("Name", "Please Enter Bank Rating");
            }            
            /// validation will come here 
            if (ModelState.IsValid)
            {
                int s = 100;
                string user_session = this.HttpContext.Session["username_session"].ToString();
                string constr = ConfigurationManager.ConnectionStrings["DAP_NEW"].ConnectionString;

                bool is_default = model.BANK_isdefault_bool;
                
                
                using (SqlConnection con = new SqlConnection(constr))
                {
                    con.Open();
                    // MessageBox.Show ("Connection Open ! ");
                    // cnn.Close();
                    SqlCommand cmd = new SqlCommand("SP_INSERT_BANK", con);    // to be corrected 
                    // SqlCommand cmd1 = new SqlCommand("select top 1 * from Customer_IDs where ID_NO ='" + txtCnicNo + "'", oCnn);   
                    cmd.CommandType = CommandType.StoredProcedure;
                    //cmd.Parameters.AddWithValue("@Rating_Id", model.rating_id);
                    cmd.Parameters.AddWithValue("@Bank_Name", model.Bank_Name);
                    cmd.Parameters.AddWithValue("@Bank_ShortName", model.Bank_ShortName);

                    if (model.Bank_Code == null)
                    { model.Bank_Code = ""; }
                    cmd.Parameters.AddWithValue("@Bank_Code", model.Bank_Code);
                    cmd.Parameters.AddWithValue("@BNKTYPE_ID", Int32.Parse(model.BNKTYPE_ID));
                    cmd.Parameters.AddWithValue("@RATING_ID", Int32.Parse(model.RATING_ID));
                    cmd.Parameters.AddWithValue("@BANK_STATUS", model.BANK_STATUS);
                    if (model.BANK_ADDRESSLINE1 == null)
                    { model.BANK_ADDRESSLINE1 = ""; }
                    cmd.Parameters.AddWithValue("@BANK_ADDRESSLINE1", model.BANK_ADDRESSLINE1.ToString());
                    if (model.BANK_ADDRESSLINE2 == null)
                    { model.BANK_ADDRESSLINE2 = ""; }
                    cmd.Parameters.AddWithValue("@BANK_ADDRESSLINE2", model.BANK_ADDRESSLINE2.ToString());
                    cmd.Parameters.AddWithValue("@COUNTRY_ID", Int32.Parse(model.COUNTRY_ID));
                    cmd.Parameters.AddWithValue("@STATE_ID", Int32.Parse(model.STATE_ID));
                    cmd.Parameters.AddWithValue("@CITY_ID", Int32.Parse(model.CITY_ID));
                    if (model.BANK_PHONES == null)
                    { model.BANK_PHONES = ""; }
                    cmd.Parameters.AddWithValue("@BANK_PHONES", model.BANK_PHONES.ToString());
                    if (model.BANK_FAX == null)
                    { model.BANK_FAX = ""; }
                    cmd.Parameters.AddWithValue("@BANK_FAX", model.BANK_FAX.ToString());
                    if (model.BANK_EMAIL == null)
                    { model.BANK_EMAIL = ""; }
                    cmd.Parameters.AddWithValue("@BANK_EMAIL", model.BANK_EMAIL.ToString());
                    if (model.BANK_WEBSITE == null)
                    { model.BANK_WEBSITE = ""; }
                    cmd.Parameters.AddWithValue("@BANK_WEBSITE", model.BANK_WEBSITE.ToString());
                    if (model.BANK_CONTACTPERSONNAME == null)
                    { model.BANK_CONTACTPERSONNAME = ""; }
                    cmd.Parameters.AddWithValue("@BANK_CONTACTPERSONNAME", model.BANK_CONTACTPERSONNAME.ToString());
                    if (model.BANK_CONTACTPERSONMOBILE == null)
                    { model.BANK_CONTACTPERSONMOBILE = ""; }
                    cmd.Parameters.AddWithValue("@BANK_CONTACTPERSONMOBILE", model.BANK_CONTACTPERSONMOBILE.ToString());
                    if (model.BANK_CONTACTPERSONEMAIL == null)
                    { model.BANK_CONTACTPERSONEMAIL = ""; }
                    cmd.Parameters.AddWithValue("@BANK_CONTACTPERSONEMAIL", model.BANK_CONTACTPERSONEMAIL.ToString());
                    cmd.Parameters.AddWithValue("@sUser", user_session);
                    cmd.Parameters.AddWithValue("@BANK_IsDefault", model.BANK_isdefault_bool);
                    s = cmd.ExecuteNonQuery();
                    con.Close();
                }
                return RedirectToAction("bank_setup", "Home");
            }
            else
            {
                string stid = "";
                string countryid = "";
                ///// in case of validation 
                string constr = ConfigurationManager.ConnectionStrings["DAP_NEW"].ConnectionString;
                DataTable table = new DataTable();
                using (SqlConnection con = new SqlConnection(constr))
                {
                    string query = "SELECT RATING_ID,RATING_NAME FROM CONFIG_RATING";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        SqlDataAdapter ds = new SqlDataAdapter(cmd);
                        ds.Fill(table);
                    }
                }
                List<Bank_Setup> r_list = new List<Bank_Setup>();
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    //  bankname="Anubhav",shortname="Chaudhary",rating="AA+"

                    r_list.Add(new Bank_Setup()
                    {
                        RATING_ID = table.Rows[i]["RATING_ID"].ToString(),
                        RATING_NAME = table.Rows[i]["RATING_NAME"].ToString(),
                    });
                }
                //////////////////////////////#######################################################////////////////////////
                DataTable table1 = new DataTable();
                using (SqlConnection con = new SqlConnection(constr))
                {
                    string query = "SELECT BNKTYPE_ID,BNKTYPE_NAME FROM CONFIG_BANKTYPE order by BNKTYPE_ISDEFAULT desc  ";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        SqlDataAdapter ds = new SqlDataAdapter(cmd);
                        ds.Fill(table1);
                    }
                }
                List<Bank_Setup> banktype_list = new List<Bank_Setup>();
                for (int i = 0; i < table1.Rows.Count; i++)
                {
                    //  bankname="Anubhav",shortname="Chaudhary",rating="AA+"

                    banktype_list.Add(new Bank_Setup()
                    {
                        BNKTYPE_ID = table1.Rows[i]["BNKTYPE_ID"].ToString(),
                        BNKTYPE_NAME = table1.Rows[i]["BNKTYPE_NAME"].ToString(),
                    });
                }
                //////////////////////////////#######################################################////////////////////////

                List<Bank_Setup> city_list = new List<Bank_Setup>();                
                DataTable table2_city_ddl = new DataTable();
                using (SqlConnection con = new SqlConnection(constr))
                {
      string query = 
      "select CITY_ID,CITY_NAME,COUNTRY_ID,STATE_ID from REGION_CITY WHERE CITY_ID='"+model.CITY_ID+"'";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        SqlDataAdapter ds = new SqlDataAdapter(cmd);
                        ds.Fill(table2_city_ddl);
                    }
                }
              //  List<Bank_Setup> city_list = new List<Bank_Setup>();
                for (int i = 0; i < table2_city_ddl.Rows.Count; i++)
                {
                    //  bankname="Anubhav",shortname="Chaudhary",rating="AA+"

                    city_list.Add(new Bank_Setup()
                    {
                        CITY_ID = table2_city_ddl.Rows[i]["CITY_ID"].ToString(),
                        CITY_NAME = table2_city_ddl.Rows[i]["CITY_NAME"].ToString(),
                    });
                    stid = table2_city_ddl.Rows[i]["STATE_ID"].ToString();
                    countryid = table2_city_ddl.Rows[i]["COUNTRY_ID"].ToString();
                
                }

                DataTable table2 = new DataTable();
                using (SqlConnection con = new SqlConnection(constr))
                {
                    string query =
                    "select CITY_ID,CITY_NAME,COUNTRY_ID,STATE_ID from REGION_CITY WHERE CITY_ID!='" + model.CITY_ID + "'";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        SqlDataAdapter ds = new SqlDataAdapter(cmd);
                        ds.Fill(table2);
                    }
                }
           
                for (int i = 0; i < table2.Rows.Count; i++)
                {                    //  bankname="Anubhav",shortname="Chaudhary",rating="AA+"
                    city_list.Add(new Bank_Setup()
                    {
                        CITY_ID = table2.Rows[i]["CITY_ID"].ToString(),
                        CITY_NAME = table2.Rows[i]["CITY_NAME"].ToString(),
                    });                  
                }

                //////////////////////////////#######################################################////////////////////////
                DataTable table3 = new DataTable();
                using (SqlConnection con = new SqlConnection(constr))
                {
                    string query = "select STATE_ID,STATE_NAME from REGION_STATE where STATE_ID='"+stid+"'";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        SqlDataAdapter ds = new SqlDataAdapter(cmd);
                        ds.Fill(table3);
                    }
                }
                List<Bank_Setup> state_list = new List<Bank_Setup>();
                for (int i = 0; i < table3.Rows.Count; i++)
                {
                    //  bankname="Anubhav",shortname="Chaudhary",rating="AA+"

                    state_list.Add(new Bank_Setup()
                    {
                        STATE_ID = table3.Rows[i]["STATE_ID"].ToString(),
                        STATE_NAME = table3.Rows[i]["STATE_NAME"].ToString(),
                    });
                }
                //////////////////////////////#######################################################////////////////////////
                DataTable table4 = new DataTable();
                using (SqlConnection con = new SqlConnection(constr))
                {
               string query = "select COUNTRY_ID,COUNTRY_OFFICIALNAME from REGION_COUNTRY where COUNTRY_ID='"+countryid+"'";
               using (SqlCommand cmd = new SqlCommand(query, con))
                {
                        SqlDataAdapter ds = new SqlDataAdapter(cmd);
                        ds.Fill(table4);
                }
                }
                List<Bank_Setup> country_list = new List<Bank_Setup>();
                for (int i = 0; i < table4.Rows.Count; i++)
                {
                    //  bankname="Anubhav",shortname="Chaudhary",rating="AA+"
                    country_list.Add(new Bank_Setup()
                    {
                        COUNTRY_ID = table4.Rows[i]["COUNTRY_ID"].ToString(),
                        COUNTRY_NAME = table4.Rows[i]["COUNTRY_OFFICIALNAME"].ToString(),
                    });
                }
                DataTable table5 = new DataTable();
                //string constr = ConfigurationManager.ConnectionStrings["DAP_NEW"].ConnectionString;
                using (SqlConnection con = new SqlConnection(constr))
                {
                    //string query="select RATING_ID,RATING_NAME,RATING_SHORTNAME,RATING_STATUS,RATING_ISDEFAULT from config_rating ";
                    using (SqlCommand cmd = new SqlCommand("SP_GET_STP_BANK", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        con.Open();
                        SqlDataAdapter ds = new SqlDataAdapter(cmd);
                        ds.Fill(table5);
                        con.Close();
                    }
                }
                List<Bank_Setup> bank_setup_list = new List<Bank_Setup>();
                for (int i = 0; i < table5.Rows.Count; i++)
                {

                    bank_setup_list.Add(new Bank_Setup()
                    {
                        BANK_ID = table5.Rows[i]["Bank_Id"].ToString(),
                        Bank_Name = table5.Rows[i]["Bank_Name"].ToString(),
                        Bank_ShortName = table5.Rows[i]["Bank_ShortName"].ToString(),
                        Bank_Code = table5.Rows[i]["Bank_Code"].ToString(),
                        BNKTYPE_NAME = table5.Rows[i]["BNKTYPE_NAME"].ToString(),
                        RATING_NAME = table5.Rows[i]["RATING_NAME"].ToString(),
                        BANK_STATUS = table5.Rows[i]["BANK_STATUS"].ToString(),
                        BANK_ADDRESSLINE1 = table5.Rows[i]["BANK_ADDRESSLINE1"].ToString(),
                        BANK_ADDRESSLINE2 = table5.Rows[i]["BANK_ADDRESSLINE2"].ToString(),
                        COUNTRY_NAME = table5.Rows[i]["COUNTRY_OFFICIALNAME"].ToString(),
                        STATE_NAME = table5.Rows[i]["STATE_NAME"].ToString(),
                        CITY_NAME = table5.Rows[i]["CITY_NAME"].ToString(),
                        BANK_PHONES = table5.Rows[i]["BANK_PHONES"].ToString(),
                        BANK_FAX = table5.Rows[i]["BANK_FAX"].ToString(),
                        BANK_EMAIL = table5.Rows[i]["BANK_EMAIL"].ToString(),
                        BANK_WEBSITE = table5.Rows[i]["BANK_WEBSITE"].ToString(),
                        BANK_CONTACTPERSONNAME = table5.Rows[i]["BANK_CONTACTPERSONNAME"].ToString(),
                        BANK_CONTACTPERSONMOBILE = table5.Rows[i]["BANK_CONTACTPERSONMOBILE"].ToString(),
                        BANK_CONTACTPERSONEMAIL = table5.Rows[i]["BANK_CONTACTPERSONEMAIL"].ToString(),
                   BANK_isdefault=table5.Rows[i]["ISDEFAULT"].ToString()
                    
                    });
                }
                Bank_Setup user1 = new Bank_Setup();
                user1.rating_list = r_list;
                user1.city_list = city_list;
                user1.state_list = state_list;
                user1.bank_type_list = banktype_list;
                user1.country_list = country_list;
                user1.bank_setup_list = bank_setup_list;
                return View(user1);
                // return View();
            }

        }
        // return View();
        public JsonResult edit_bank_setup(Bank_Setup model)
        {
            int s = 100;
            string user_session = this.HttpContext.Session["username_session"].ToString();
            //string accounttypeid = model.ACCTYPE_ID.ToString();
            //string accounttypename = model.ACCTYPE_NAME.ToString();
            //string accountshortname = model.ACCTYPE_SHORTNAME.ToString();
            //string accounttypestatus = model.ACCTYPE_STATUS.ToString();
            //string ratingisdefault = model.ACCTYPE_ISDEFAULT.ToString();
            //if (ratingisdefault == "TRUE")
            //{
            //    ratingisdefault = "1";
            //}
            //if (ratingisdefault == "FALSE")
            //{
            //    ratingisdefault = "0";
            //}


            string isdefault = model.BANK_isdefault.ToString();

            string constr = ConfigurationManager.ConnectionStrings["DAP_NEW"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {

                if (string.IsNullOrEmpty(model.BANK_ADDRESSLINE1))
                {
                    model.BANK_ADDRESSLINE1 = "";
                }
                if (string.IsNullOrEmpty(model.BANK_ADDRESSLINE2))
                {
                    model.BANK_ADDRESSLINE2 = "";
                }

                if (string.IsNullOrEmpty(model.BANK_WEBSITE))
                {
                    model.BANK_WEBSITE = "";
                }

                if (string.IsNullOrEmpty(model.STATE_ID))
                {
                    model.STATE_ID = "1";
                }

                if (string.IsNullOrEmpty(model.BANK_CONTACTPERSONNAME))
                {
                  model.BANK_CONTACTPERSONNAME = "";
                }
                if (string.IsNullOrEmpty(model.BANK_CONTACTPERSONEMAIL))
                {
                    model.BANK_CONTACTPERSONEMAIL = "";
                }
                if (string.IsNullOrEmpty(model.BANK_CONTACTPERSONMOBILE))
                {
                    model.BANK_CONTACTPERSONMOBILE = "";
                }
                if (string.IsNullOrEmpty(model.BANK_FAX))
                {
                    model.BANK_FAX = "";
                }

                //////////////////////////////////// changes from fin dept //////////////////////////////////
                model.BANK_EMAIL = "";
                model.Bank_Code = "";
                model.BANK_PHONES = "";

                ////////////////////////////////////  //////////////////////////////////


                con.Open();
                // MessageBox.Show ("Connection Open ! ");
                // cnn.Close();
                SqlCommand cmd = new SqlCommand("SP_UPDATE_Bank", con);    // to be corrected 
                // SqlCommand cmd1 = new SqlCommand("select top 1 * from Customer_IDs where ID_NO ='" + txtCnicNo + "'", oCnn);   
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Bank_Id", model.BANK_ID);
                cmd.Parameters.AddWithValue("@Bank_Name", model.Bank_Name);
                cmd.Parameters.AddWithValue("@Bank_ShortName", model.Bank_ShortName);
                cmd.Parameters.AddWithValue("@Bank_Code", model.Bank_Code);
                cmd.Parameters.AddWithValue("@BNKTYPE_ID", model.BNKTYPE_ID);
                cmd.Parameters.AddWithValue("@RATING_ID", model.RATING_ID);
                cmd.Parameters.AddWithValue("@BANK_STATUS", model.BANK_STATUS);
                cmd.Parameters.AddWithValue("@BANK_ADDRESSLINE1", model.BANK_ADDRESSLINE1);
                cmd.Parameters.AddWithValue("@BANK_ADDRESSLINE2", model.BANK_ADDRESSLINE2);
                cmd.Parameters.AddWithValue("@COUNTRY_ID", model.COUNTRY_ID);
                cmd.Parameters.AddWithValue("@STATE_ID", model.STATE_ID);
                cmd.Parameters.AddWithValue("@CITY_ID", model.CITY_ID);
                cmd.Parameters.AddWithValue("@BANK_PHONES", model.BANK_PHONES);
                cmd.Parameters.AddWithValue("@BANK_FAX", model.BANK_FAX);
                cmd.Parameters.AddWithValue("@BANK_EMAIL", model.BANK_EMAIL);
                cmd.Parameters.AddWithValue("@BANK_WEBSITE", model.BANK_WEBSITE);
                cmd.Parameters.AddWithValue("@BANK_CONTACTPERSONNAME", model.BANK_CONTACTPERSONNAME);
                cmd.Parameters.AddWithValue("@BANK_CONTACTPERSONMOBILE", model.BANK_CONTACTPERSONMOBILE);
                cmd.Parameters.AddWithValue("@BANK_CONTACTPERSONEMAIL", model.BANK_CONTACTPERSONEMAIL);
                cmd.Parameters.AddWithValue("@sUser", user_session);
                cmd.Parameters.AddWithValue("@BANK_IsDefault", isdefault);
                
                s = cmd.ExecuteNonQuery();
                con.Close();
            }

            string message1 = "Successful";
            string message2 = "unsuccessful";
            if (s == 0)
            {
                return Json(message2, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(message1, JsonRequestBehavior.AllowGet);
            }

            //return View();

        }
        [HttpPost]
        public ActionResult ExportData_bank_setup() 
        {
            string txtFile = string.Empty;
            DataTable table = new DataTable();
            string constr = ConfigurationManager.ConnectionStrings["DAP_NEW"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                //string query="select RATING_ID,RATING_NAME,RATING_SHORTNAME,RATING_STATUS,RATING_ISDEFAULT from config_rating ";
                using (SqlCommand cmd = new SqlCommand("SP_GET_STP_BANK", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    con.Open();
                    SqlDataAdapter ds = new SqlDataAdapter(cmd);
                    ds.Fill(table);
                    con.Close();
                }
            }
            List<Bank_Setup_pdf> bank_list = new List<Bank_Setup_pdf>();
            for (int i = 0; i < table.Rows.Count; i++)
            {
                bank_list.Add(new Bank_Setup_pdf()
                {
                    //Serial = table.Rows[i]["ACCTYPE_ID"].ToString(),
                    Name = table.Rows[i]["BANK_NAME"].ToString(),
                    ShortName = table.Rows[i]["BANK_SHORTNAME"].ToString(),
                    Status = table.Rows[i]["BANK_STATUS"].ToString(),
                    //BankCode = table.Rows[i]["Bank_Code"].ToString(),
                    BankType = table.Rows[i]["BNKTYPE_NAME"].ToString(),
                    //IsDefault = table.Rows[i]["ACCTYPE_ISDEFAULT"].ToString(),
                    Rating = table.Rows[i]["RATING_NAME"].ToString()
                    //CreatedBy = table.Rows[i]["ACCTYPE_CREATEDBY"].ToString(),
                    //CreatedOn = table.Rows[i]["ACCTYPE_CREATEDON"].ToString(),
                    //ModifiedBy = table.Rows[i]["ACCTYPE_MODIFIEDBY"].ToString(),
                    //ModifiedOn = table.Rows[i]["ACCTYPE_MODIFIEDON"].ToString(),
                });
            }
            GridView gv = new GridView();
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "attachment;filename=Bank_Setup_Report.pdf");
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            StringWriter sw = new StringWriter();
            HtmlTextWriter hw = new HtmlTextWriter(sw);
            gv.DataSource = bank_list;
            gv.AllowPaging = false;
            gv.DataBind();
            //for (int i = 0; i < gv.Columns.Count; i++)
            //{
            //    //   gv.Columns[0].HeaderText = "Serial";
            //    gv.Columns[i].ItemStyle.Width = 400;
            //    gv.Columns[i].ItemStyle.Wrap = false;
            //}    
            gv.HeaderRow.Style.Add("width", "300%");
            gv.HeaderRow.Style.Add("font-size", "7px");
            gv.Style.Add("text-decoration", "none");
            gv.Style.Add("font-family", "Arial, Helvetica, sans-serif;");
            gv.Style.Add("font-size", "7px");
            //gv.Style.Add("width", "300%");         
            gv.RenderControl(hw);
            StringReader sr = new StringReader(sw.ToString());
            Document pdfDoc = new Document(PageSize.A4.Rotate(), 30f, 30f, 30f, 30f);
            HTMLWorker htmlparser = new HTMLWorker(pdfDoc);
            PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
            pdfDoc.Open();
            htmlparser.Parse(sr);//this is the error line
            pdfDoc.Close();
            Response.Write(pdfDoc);
            Response.End();
            //return RedirectToAction("account_type", "Home");
            return RedirectToAction("bank_setup", "Home");                
        }        
        public ActionResult branch_setup()
        {

             if (this.HttpContext.Session["username_session"] != null)
    {

            string constr = ConfigurationManager.ConnectionStrings["DAP_NEW"].ConnectionString;
            DataTable table = new DataTable();
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = "SELECT * FROM STP_BANKBRANCH sbb left join REGION_COUNTRY co   on co.COUNTRY_ID=sbb.COUNTRY_ID ";
                query = query + "left join REGION_CITY ci   on ci.CITY_ID=sbb.CITY_ID ";
                query = query + "left join REGION_STATE st  on st.STATE_ID=sbb.STATE_ID ";
                query = query + "left join STP_BANK sb on sb.BANK_ID=sbb.BANK_ID ";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    SqlDataAdapter ds = new SqlDataAdapter(cmd);
                    ds.Fill(table);
                }
            }
            List<Branch_Setup> branch_setup_list = new List<Branch_Setup>();
            for (int i = 0; i < table.Rows.Count; i++)
            {
                branch_setup_list.Add(new Branch_Setup()
                {
                    BBR_ID = table.Rows[i]["BBR_ID"].ToString(),
                    BANK_ID = table.Rows[i]["BANK_ID"].ToString(),
                    BANK_NAME = table.Rows[i]["BANK_NAME"].ToString(),
                    BBR_NAME = table.Rows[i]["BBR_NAME"].ToString(),
                    BBR_CODE = table.Rows[i]["BBR_CODE"].ToString(),
                    BBR_ISISLAMIC = table.Rows[i]["BBR_ISISLAMIC"].ToString(),
                    BBR_SWIFTCODE = table.Rows[i]["BBR_SWIFTCODE"].ToString(),
                    BBR_STATUS = table.Rows[i]["BBR_STATUS"].ToString(),
                    BBR_ADDRESSLINE1 = table.Rows[i]["BBR_ADDRESSLINE1"].ToString(),
                    BBR_ADDRESSLINE2 = table.Rows[i]["BBR_ADDRESSLINE2"].ToString(),
                    COUNTRY_ID = table.Rows[i]["COUNTRY_ID"].ToString(),
                    COUNTRY_NAME = table.Rows[i]["COUNTRY_OFFICIALNAME"].ToString(),
                    STATE_ID = table.Rows[i]["STATE_ID"].ToString(),
                    STATE_NAME = table.Rows[i]["STATE_NAME"].ToString(),
                    CITY_ID = table.Rows[i]["CITY_ID"].ToString(),
                    CITY_NAME = table.Rows[i]["CITY_NAME"].ToString(),
                    BBR_PHONES = table.Rows[i]["BBR_PHONES"].ToString(),
                    BBR_FAX = table.Rows[i]["BBR_FAX"].ToString(),
                    //  BBR_EMAIL = table.Rows[i]["BBR_EMAIL"].ToString(),
                    BBR_CONTACTPERSONNAME = table.Rows[i]["BBR_CONTACTPERSONNAME"].ToString(),
                    BBR_CONTACTPERSONMOBILE = table.Rows[i]["BBR_CONTACTPERSONMOBILE"].ToString(),
                    BBR_CONTACTPERSONEMAIL = table.Rows[i]["BBR_CONTACTPERSONEMAIL"].ToString(),
                    BBR_isdefault = table.Rows[i]["ISDEFAULT"].ToString()
                
                });
            }
            //////////////////////////////#######################################################////////////////////////


            // string constr = ConfigurationManager.ConnectionStrings["DAP_NEW"].ConnectionString;
            DataTable table2 = new DataTable();
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = "select CITY_ID,CITY_NAME from REGION_CITY";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    SqlDataAdapter ds = new SqlDataAdapter(cmd);
                    ds.Fill(table2); ;
                }
            }
            List<Branch_Setup> city_list = new List<Branch_Setup>();
            for (int i = 0; i < table2.Rows.Count; i++)
            {
                //  bankname="Anubhav",shortname="Chaudhary",rating="AA+"

                city_list.Add(new Branch_Setup()
                {
                    CITY_ID = table2.Rows[i]["CITY_ID"].ToString(),
                    CITY_NAME = table2.Rows[i]["CITY_NAME"].ToString(),
                });
            }
            //////////////////////////////#######################################################////////////////////////
            DataTable table3 = new DataTable();
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = "select STATE_ID,STATE_NAME from REGION_STATE";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    SqlDataAdapter ds = new SqlDataAdapter(cmd);
                    ds.Fill(table3);
                }
            }
            List<Branch_Setup> state_list = new List<Branch_Setup>();
            for (int i = 0; i < table3.Rows.Count; i++)
            {
                //  bankname="Anubhav",shortname="Chaudhary",rating="AA+"

                state_list.Add(new Branch_Setup()
                {
                    STATE_ID = table3.Rows[i]["STATE_ID"].ToString(),
                    STATE_NAME = table3.Rows[i]["STATE_NAME"].ToString(),
                });
            }
            //////////////////////////////#######################################################////////////////////////
            DataTable table4 = new DataTable();
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = "select COUNTRY_ID,COUNTRY_OFFICIALNAME from REGION_COUNTRY";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    SqlDataAdapter ds = new SqlDataAdapter(cmd);
                    ds.Fill(table4);
                }
            }
            List<Branch_Setup> country_list = new List<Branch_Setup>();
            for (int i = 0; i < table4.Rows.Count; i++)
            {
                //  bankname="Anubhav",shortname="Chaudhary",rating="AA+"

                country_list.Add(new Branch_Setup()
                {
                    COUNTRY_ID = table4.Rows[i]["COUNTRY_ID"].ToString(),
                    COUNTRY_NAME = table4.Rows[i]["COUNTRY_OFFICIALNAME"].ToString(),
                });
            }

            //////////////////////////////////////////#########################################/////////////////////////////////
            DataTable table5 = new DataTable();
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = "SELECT BANK_ID, BANK_NAME FROM STP_BANK";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    SqlDataAdapter ds = new SqlDataAdapter(cmd);
                    ds.Fill(table5);
                }
            }
            List<Branch_Setup> bank_list = new List<Branch_Setup>();
            for (int i = 0; i < table5.Rows.Count; i++)
            {
                //  bankname="Anubhav",shortname="Chaudhary",rating="AA+"
                bank_list.Add(new Branch_Setup()
                {
                    BANK_ID = table5.Rows[i]["BANK_ID"].ToString(),
                    BANK_NAME = table5.Rows[i]["BANK_NAME"].ToString(),
                });
            }


            ////////////////////////////////////////#########################################////////////////////

            Branch_Setup user1 = new Branch_Setup();
            //user1.rating_list = r_list;
            user1.branch_setup_list = branch_setup_list;
            user1.bank_setup_list = bank_list;
            user1.city_list = city_list;
            user1.state_list = state_list;
            //user1.bank_type_list = banktype_list;
            user1.country_list = country_list;
            return View(user1);

    }
             else
             {
                 return RedirectToAction("login", "Home");

             }	
             
             
             }
        [HttpPost]
        public ActionResult branch_setup(Branch_Setup model)
        {
            string cid = model.CITY_ID;


            if (string.IsNullOrEmpty(model.BANK_ID))
            {
                ModelState.AddModelError("Name", "Please Enter Bank Name");
            }
            if (string.IsNullOrEmpty(model.BBR_NAME))
            {
                ModelState.AddModelError("Name", "Please Enter Branch Name");
            }
            //if (string.IsNullOrEmpty(model.BBR_CODE))
            //{
            //    ModelState.AddModelError("Name", "Please Enter Branch Code");
            //}
            //if (string.IsNullOrEmpty(model.BBR_SWIFTCODE))
            //{
            //    ModelState.AddModelError("Name", "Please Enter Branch swift Code");
            //}
           
            if (!string.IsNullOrEmpty(model.BBR_CONTACTPERSONEMAIL))
            {
                string emailRegex = @"^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$";
                Regex re = new Regex(emailRegex);
                if (!re.IsMatch(model.BBR_CONTACTPERSONEMAIL))
                {
                    ModelState.AddModelError("Email", "Please Enter Correct Email Address");
                }
            }

            if (string.IsNullOrEmpty(model.BBR_SWIFTCODE))
            {
                model.BBR_SWIFTCODE = "";
            }
            if (string.IsNullOrEmpty(model.BBR_CODE))
            {
                model.BBR_CODE = "";
            }      
            
            
            if (ModelState.IsValid)
            {
                int s = 100;
                string user_session = this.HttpContext.Session["username_session"].ToString();
                string constr = ConfigurationManager.ConnectionStrings["DAP_NEW"].ConnectionString;
                using (SqlConnection con = new SqlConnection(constr))
                {
                    con.Open();
                    // MessageBox.Show ("Connection Open ! ");
                    // cnn.Close();
                    SqlCommand cmd = new SqlCommand("SP_INSERT_BANKBRANCH", con);    // to be corrected 
                    // SqlCommand cmd1 = new SqlCommand("select top 1 * from Customer_IDs where ID_NO ='" + txtCnicNo + "'", oCnn);   
                    cmd.CommandType = CommandType.StoredProcedure;
                    //cmd.Parameters.AddWithValue("@Rating_Id", model.rating_id);
                    cmd.Parameters.AddWithValue("@BBR_Name", model.BBR_NAME);
                    cmd.Parameters.AddWithValue("@BBR_Code", model.BBR_CODE);
                    cmd.Parameters.AddWithValue("@BBR_ISSLAMIC", model.BBR_ISISLAMIC_BOOL);
                    cmd.Parameters.AddWithValue("@BBR_SWIFT", model.BBR_SWIFTCODE.ToString());
                    cmd.Parameters.AddWithValue("@Bank_ID", Int32.Parse(model.BANK_ID));
                    cmd.Parameters.AddWithValue("@BBR_STATUS", model.BBR_STATUS);
                    if (model.BBR_ADDRESSLINE1 == null)
                    { model.BBR_ADDRESSLINE1 = ""; }
                    cmd.Parameters.AddWithValue("@BBR_ADDRESSLINE1", model.BBR_ADDRESSLINE1.ToString());
                    if (model.BBR_ADDRESSLINE2 == null)
                    { model.BBR_ADDRESSLINE2 = ""; }
                    cmd.Parameters.AddWithValue("@BBR_ADDRESSLINE2", model.BBR_ADDRESSLINE2.ToString());
                    cmd.Parameters.AddWithValue("@BBR_COUNTRY_ID", Int32.Parse(model.COUNTRY_ID));
                    cmd.Parameters.AddWithValue("@BBR_STATE_ID", Int32.Parse(model.STATE_ID));
                    cmd.Parameters.AddWithValue("@BBR_CITY_ID", Int32.Parse(model.CITY_ID));
                    if (model.BBR_PHONES == null)
                    { model.BBR_PHONES = ""; }
                    cmd.Parameters.AddWithValue("@BBR_BANK_PHONES", model.BBR_PHONES.ToString());
                    if (model.BBR_FAX == null)
                    { model.BBR_FAX = ""; }
                    cmd.Parameters.AddWithValue("@BBR_BANK_FAX", model.BBR_FAX.ToString());
                    if (model.BBR_EMAIL == null)
                    { model.BBR_EMAIL = ""; }
                    cmd.Parameters.AddWithValue("@BBR_BANK_EMAIL", model.BBR_EMAIL.ToString());
                    //if (model.BANK_WEBSITE == null)
                    //{ model.BANK_WEBSITE = ""; }
                    //cmd.Parameters.AddWithValue("@BANK_WEBSITE", model.BANK_WEBSITE.ToString());
                    if (model.BBR_CONTACTPERSONNAME == null)
                    { model.BBR_CONTACTPERSONNAME = ""; }
                    cmd.Parameters.AddWithValue("@BBR_CONTACTPERSONNAME", model.BBR_CONTACTPERSONNAME.ToString());
                    if (model.BBR_CONTACTPERSONMOBILE == null)
                    { model.BBR_CONTACTPERSONMOBILE = ""; }
                    cmd.Parameters.AddWithValue("@BBR_CONTACTPERSONMOBILE", model.BBR_CONTACTPERSONMOBILE.ToString());
                    if (model.BBR_CONTACTPERSONEMAIL == null)
                    { model.BBR_CONTACTPERSONEMAIL = ""; }
                    cmd.Parameters.AddWithValue("@BBR_CONTACTPERSONEMAIL", model.BBR_CONTACTPERSONEMAIL.ToString());
                    cmd.Parameters.AddWithValue("@sUser", user_session);
                    cmd.Parameters.AddWithValue("@BBR_IsDefault", model.BBR_isdefault_bool);


                    s = cmd.ExecuteNonQuery();
                    con.Close();
                }
                return RedirectToAction("branch_setup", "Home");
            }
            else
            {
                string constr = ConfigurationManager.ConnectionStrings["DAP_NEW"].ConnectionString;
                DataTable table = new DataTable();
                using (SqlConnection con = new SqlConnection(constr))
                {
                    string query = "SELECT * FROM STP_BANKBRANCH sbb left join REGION_COUNTRY co   on co.COUNTRY_ID=sbb.COUNTRY_ID ";
                    query = query + "left join REGION_CITY ci   on ci.CITY_ID=sbb.CITY_ID ";
                    query = query + "left join REGION_STATE st  on st.STATE_ID=sbb.STATE_ID ";
                    query = query + "left join STP_BANK sb on sb.BANK_ID=sbb.BANK_ID ";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        SqlDataAdapter ds = new SqlDataAdapter(cmd);
                        ds.Fill(table);
                    }
                }
                List<Branch_Setup> branch_setup_list = new List<Branch_Setup>();
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    branch_setup_list.Add(new Branch_Setup()
                    {
                        BBR_ID = table.Rows[i]["BBR_ID"].ToString(),
                        BANK_ID = table.Rows[i]["BANK_ID"].ToString(),
                        BANK_NAME = table.Rows[i]["BANK_NAME"].ToString(),
                        BBR_NAME = table.Rows[i]["BBR_NAME"].ToString(),
                        BBR_CODE = table.Rows[i]["BBR_CODE"].ToString(),
                        BBR_ISISLAMIC = table.Rows[i]["BBR_ISISLAMIC"].ToString(),
                        BBR_SWIFTCODE = table.Rows[i]["BBR_SWIFTCODE"].ToString(),
                        BBR_STATUS = table.Rows[i]["BBR_STATUS"].ToString(),
                        BBR_ADDRESSLINE1 = table.Rows[i]["BBR_ADDRESSLINE1"].ToString(),
                        BBR_ADDRESSLINE2 = table.Rows[i]["BBR_ADDRESSLINE2"].ToString(),
                        COUNTRY_ID = table.Rows[i]["COUNTRY_ID"].ToString(),
                        COUNTRY_NAME = table.Rows[i]["COUNTRY_OFFICIALNAME"].ToString(),
                        STATE_ID = table.Rows[i]["STATE_ID"].ToString(),
                        STATE_NAME = table.Rows[i]["STATE_NAME"].ToString(),
                        CITY_ID = table.Rows[i]["CITY_ID"].ToString(),
                        CITY_NAME = table.Rows[i]["CITY_NAME"].ToString(),
                        BBR_PHONES = table.Rows[i]["BBR_PHONES"].ToString(),
                        BBR_FAX = table.Rows[i]["BBR_FAX"].ToString(),
                        //  BBR_EMAIL = table.Rows[i]["BBR_EMAIL"].ToString(),
                        BBR_CONTACTPERSONNAME = table.Rows[i]["BBR_CONTACTPERSONNAME"].ToString(),
                        BBR_CONTACTPERSONMOBILE = table.Rows[i]["BBR_CONTACTPERSONMOBILE"].ToString(),
                        BBR_CONTACTPERSONEMAIL = table.Rows[i]["BBR_CONTACTPERSONEMAIL"].ToString(),
                        BBR_isdefault = table.Rows[i]["ISDEFAULT"].ToString()
                    });
                }
                //////////////////////////////#######################################################////////////////////////


                string stid = "";
                string countryid = "";

                List<Branch_Setup> city_list = new List<Branch_Setup>();
                // string constr = ConfigurationManager.ConnectionStrings["DAP_NEW"].ConnectionString;
                DataTable table2_city_ddl = new DataTable();
                using (SqlConnection con = new SqlConnection(constr))
                {
    string query = "select CITY_ID,CITY_NAME,COUNTRY_ID,STATE_ID from REGION_CITY where CITY_ID='"+model.CITY_ID+"'";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        SqlDataAdapter ds = new SqlDataAdapter(cmd);
                        ds.Fill(table2_city_ddl);
                    }
                }

                for (int i = 0; i < table2_city_ddl.Rows.Count; i++)
                {
                    //  bankname="Anubhav",shortname="Chaudhary",rating="AA+"

                    city_list.Add(new Branch_Setup()
                    {
                        CITY_ID = table2_city_ddl.Rows[i]["CITY_ID"].ToString(),
                        CITY_NAME = table2_city_ddl.Rows[i]["CITY_NAME"].ToString(),
                    });

                    stid = table2_city_ddl.Rows[i]["STATE_ID"].ToString();
                    countryid = table2_city_ddl.Rows[i]["COUNTRY_ID"].ToString();
                
                }
               
                //////////////////////////////////////////////////////////////////

                DataTable table2 = new DataTable();
                using (SqlConnection con = new SqlConnection(constr))
                {
                    string query = "select CITY_ID,CITY_NAME,COUNTRY_ID,STATE_ID from REGION_CITY where CITY_ID!='" + model.CITY_ID + "'";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        SqlDataAdapter ds = new SqlDataAdapter(cmd);
                        ds.Fill(table2);
                    }
                }

                for (int i = 0; i < table2.Rows.Count; i++)
                {
                    //  bankname="Anubhav",shortname="Chaudhary",rating="AA+"

                    city_list.Add(new Branch_Setup()
                    {
                        CITY_ID = table2.Rows[i]["CITY_ID"].ToString(),
                        CITY_NAME = table2.Rows[i]["CITY_NAME"].ToString(),
                    });

                }
    
                
                //////////////////////////////#######################################################////////////////////////
                DataTable table3 = new DataTable();
                using (SqlConnection con = new SqlConnection(constr))
                {
                    string query = "select STATE_ID,STATE_NAME from REGION_STATE WHERE STATE_ID='" + stid+"'";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        SqlDataAdapter ds = new SqlDataAdapter(cmd);
                        ds.Fill(table3);
                    }
                }
                List<Branch_Setup> state_list = new List<Branch_Setup>();
                for (int i = 0; i < table3.Rows.Count; i++)
                {
                    //  bankname="Anubhav",shortname="Chaudhary",rating="AA+"

                    state_list.Add(new Branch_Setup()
                    {
                        STATE_ID = table3.Rows[i]["STATE_ID"].ToString(),
                        STATE_NAME = table3.Rows[i]["STATE_NAME"].ToString(),
                    });
                }
                //////////////////////////////#######################################################////////////////////////
                DataTable table4 = new DataTable();
                using (SqlConnection con = new SqlConnection(constr))
                {
   string query = 
       "select COUNTRY_ID,COUNTRY_OFFICIALNAME from REGION_COUNTRY WHERE COUNTRY_ID='"+countryid+"'";
                using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        SqlDataAdapter ds = new SqlDataAdapter(cmd);
                        ds.Fill(table4);
                    }
                }
                List<Branch_Setup> country_list = new List<Branch_Setup>();
                for (int i = 0; i < table4.Rows.Count; i++)
                {
                    //  bankname="Anubhav",shortname="Chaudhary",rating="AA+"

                    country_list.Add(new Branch_Setup()
                    {
                        COUNTRY_ID = table4.Rows[i]["COUNTRY_ID"].ToString(),
                        COUNTRY_NAME = table4.Rows[i]["COUNTRY_OFFICIALNAME"].ToString(),
                    });
                }

                //////////////////////////////////////////#########################################/////////////////////////////////
                DataTable table5 = new DataTable();
                using (SqlConnection con = new SqlConnection(constr))
                {
                    string query = "SELECT BANK_ID, BANK_NAME FROM STP_BANK";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        SqlDataAdapter ds = new SqlDataAdapter(cmd);
                        ds.Fill(table5);
                    }
                }
                List<Branch_Setup> bank_list = new List<Branch_Setup>();
                for (int i = 0; i < table5.Rows.Count; i++)
                {
                    //  bankname="Anubhav",shortname="Chaudhary",rating="AA+"
                    bank_list.Add(new Branch_Setup()
                    {
                        BANK_ID = table5.Rows[i]["BANK_ID"].ToString(),
                        BANK_NAME = table5.Rows[i]["BANK_NAME"].ToString(),
                    });
                }


                ////////////////////////////////////////#########################################////////////////////

                Branch_Setup user1 = new Branch_Setup();
                //user1.rating_list = r_list;
                user1.branch_setup_list = branch_setup_list;
                user1.bank_setup_list = bank_list;
                user1.city_list = city_list;
                user1.state_list = state_list;
                //user1.bank_type_list = banktype_list;
                user1.country_list = country_list;
                return View(user1);

            }

            /////////////////////   work needs to be done for validation code ////////////////////////////       
            //return View();
        }
        //// works needs to be done for edit function 
        public JsonResult edit_branch_setup(Branch_Setup model)            // changs needs to be done here 
        {
            int s = 100;
            string user_session = this.HttpContext.Session["username_session"].ToString();
            //string accounttypeid = model.ACCTYPE_ID.ToString();
            //string accounttypename = model.ACCTYPE_NAME.ToString();
            //string accountshortname = model.ACCTYPE_SHORTNAME.ToString();
            //string accounttypestatus = model.ACCTYPE_STATUS.ToString();
            //string ratingisdefault = model.ACCTYPE_ISDEFAULT.ToString();
            //if (ratingisdefault == "TRUE")
            //{
            //    ratingisdefault = "1";
            //}
            //if (ratingisdefault == "FALSE")
            //{
            //    ratingisdefault = "0";
            //}


            if (string.IsNullOrEmpty(model.BBR_SWIFTCODE))
            {
                model.BBR_SWIFTCODE = "";
            }

            if (string.IsNullOrEmpty(model.BBR_CODE))
            {
                model.BBR_CODE = "";
            }        

            string constr = ConfigurationManager.ConnectionStrings["DAP_NEW"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {

                con.Open();
                // MessageBox.Show ("Connection Open ! ");
                // cnn.Close();
                SqlCommand cmd = new SqlCommand("SP_UPDATE_BankBranch", con);    // to be corrected 
                // SqlCommand cmd1 = new SqlCommand("select top 1 * from Customer_IDs where ID_NO ='" + txtCnicNo + "'", oCnn);   
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@BBR_ID", model.BBR_ID);
                cmd.Parameters.AddWithValue("@Bank_Id", model.BANK_ID);
                cmd.Parameters.AddWithValue("@BBR_Name", model.BBR_NAME);
                cmd.Parameters.AddWithValue("@BBR_Code", model.BBR_CODE);
                cmd.Parameters.AddWithValue("@BBR_ISSLAMIC", model.BBR_ISISLAMIC_BOOL);
                cmd.Parameters.AddWithValue("@BBR_SWIFT", model.BBR_SWIFTCODE.ToString());
                cmd.Parameters.AddWithValue("@BBR_STATUS", model.BBR_STATUS);
                //  cmd.Parameters.AddWithValue("", model.BANK_STATUS);

                if (model.BBR_ADDRESSLINE1 == null)
                { model.BBR_ADDRESSLINE1 = ""; }

                if (model.BBR_ADDRESSLINE2 == null)
                { model.BBR_ADDRESSLINE2 = ""; }

                if (model.BBR_CONTACTPERSONNAME == null)
                { model.BBR_CONTACTPERSONNAME = ""; }

                cmd.Parameters.AddWithValue("@BBR_ADDRESSLINE1", model.BBR_ADDRESSLINE1);
                cmd.Parameters.AddWithValue("@BBR_ADDRESSLINE2", model.BBR_ADDRESSLINE2);
                cmd.Parameters.AddWithValue("@BBR_COUNTRY_ID", model.COUNTRY_ID);
                cmd.Parameters.AddWithValue("@BBR_STATE_ID", model.STATE_ID);
                cmd.Parameters.AddWithValue("@BBR_CITY_ID", model.CITY_ID);
                cmd.Parameters.AddWithValue("@BBR_BANK_PHONES", model.BBR_PHONES);
                cmd.Parameters.AddWithValue("@BBR_BANK_FAX", model.BBR_FAX);
                if (model.BBR_EMAIL == null)
                { model.BBR_EMAIL = ""; }
                cmd.Parameters.AddWithValue("@BBR_BANK_EMAIL", model.BBR_EMAIL);
                //cmd.Parameters.AddWithValue("@BANK_WEBSITE", model.BB);
                cmd.Parameters.AddWithValue("@BBR_CONTACTPERSONNAME", model.BBR_CONTACTPERSONNAME);
                if (model.BBR_CONTACTPERSONMOBILE == null)
                { model.BBR_CONTACTPERSONMOBILE = ""; }

                if (model.BBR_CONTACTPERSONEMAIL == null)
                { model.BBR_CONTACTPERSONEMAIL = ""; }
                cmd.Parameters.AddWithValue("@BBR_CONTACTPERSONMOBILE", model.BBR_CONTACTPERSONMOBILE);
                cmd.Parameters.AddWithValue("@BBR_CONTACTPERSONEMAIL", model.BBR_CONTACTPERSONEMAIL);
                cmd.Parameters.AddWithValue("@sUser", user_session);
                cmd.Parameters.AddWithValue("@BBR_IsDefault", model.BBR_isdefault.ToString());
                s = cmd.ExecuteNonQuery();
                con.Close();
            }
            string message1 = "Successful";
            string message2 = "unsuccessful";
            if (s == 0)
            {
                return Json(message2, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(message1, JsonRequestBehavior.AllowGet);
            }

            //return View();
        }
        [HttpPost]
        public ActionResult ExportData_branch_setup()
        {
            string txtFile = string.Empty;
            DataTable table = new DataTable();
            string constr = ConfigurationManager.ConnectionStrings["DAP_NEW"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = "SELECT * FROM STP_BANKBRANCH sbb left join REGION_COUNTRY co   on co.COUNTRY_ID=sbb.COUNTRY_ID ";
                query = query + "left join REGION_CITY ci   on ci.CITY_ID=sbb.CITY_ID ";
                query = query + "left join REGION_STATE st  on st.STATE_ID=sbb.STATE_ID ";
                query = query + "left join STP_BANK sb on sb.BANK_ID=sbb.BANK_ID ";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    SqlDataAdapter ds = new SqlDataAdapter(cmd);
                    ds.Fill(table);
                }
            }
            List<Branch_Setup_pdf> branch_list = new List<Branch_Setup_pdf>();
            for (int i = 0; i < table.Rows.Count; i++)
            {
                branch_list.Add(new Branch_Setup_pdf()
                {
                    BankName = table.Rows[i]["BANK_NAME"].ToString(),
                    Name = table.Rows[i]["BBR_NAME"].ToString(),
                    BranchCode = table.Rows[i]["BBR_CODE"].ToString(),
                    IsIslamaic = table.Rows[i]["BBR_ISISLAMIC"].ToString(),
                    SwiftCode = table.Rows[i]["BBR_SWIFTCODE"].ToString(),
                    Status = table.Rows[i]["BBR_STATUS"].ToString(),
                    Address =  table.Rows[i]["BBR_ADDRESSLINE1"].ToString()
                });
            }
            GridView gv = new GridView();
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "attachment;filename=Bank_Setup_Report.pdf");
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            StringWriter sw = new StringWriter();
            HtmlTextWriter hw = new HtmlTextWriter(sw);
            gv.DataSource = branch_list;
            gv.AllowPaging = false;
            gv.DataBind();
            //for (int i = 0; i < gv.Columns.Count; i++)
            //{
            //    //   gv.Columns[0].HeaderText = "Serial";
            //    gv.Columns[i].ItemStyle.Width = 400;
            //    gv.Columns[i].ItemStyle.Wrap = false;
            //}    
            gv.HeaderRow.Style.Add("width", "300%");
            gv.HeaderRow.Style.Add("font-size", "7px");
            gv.Style.Add("text-decoration", "none");
            gv.Style.Add("font-family", "Arial, Helvetica, sans-serif;");
            gv.Style.Add("font-size", "7px");
            //gv.Style.Add("width", "300%");         
            gv.RenderControl(hw);
            StringReader sr = new StringReader(sw.ToString());
            Document pdfDoc = new Document(PageSize.A4.Rotate(), 30f, 30f, 30f, 30f);
            HTMLWorker htmlparser = new HTMLWorker(pdfDoc);
            PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
            pdfDoc.Open();
            htmlparser.Parse(sr);//this is the error line
            pdfDoc.Close();
            Response.Write(pdfDoc);
            Response.End();
            //return RedirectToAction("account_type", "Home");            
            return RedirectToAction("branch_setup", "Home");        
        }     
        public ActionResult fund_setup()
        {

            if (this.HttpContext.Session["username_session"] != null)
            {
                string constr = ConfigurationManager.ConnectionStrings["DAP_T24"].ConnectionString;
                string constr_dap = ConfigurationManager.ConnectionStrings["DAP_NEW"].ConnectionString;

                DataTable table = new DataTable();
                using (SqlConnection con = new SqlConnection(constr))
                {
                    string query = "SELECT * FROM FUND ";
                    //query = query + "left join REGION_CITY ci   on ci.CITY_ID=sbb.CITY_ID ";
                    //query = query + "left join REGION_STATE st  on st.STATE_ID=sbb.STATE_ID ";
                    //query = query + "left join STP_BANK sb on sb.BANK_ID=sbb.BANK_ID ";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        SqlDataAdapter ds = new SqlDataAdapter(cmd);
                        ds.Fill(table);
                    }
                }
                List<Fund_Setup> fund_tidlist = new List<Fund_Setup>();
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    fund_tidlist.Add(new Fund_Setup()
                    {
                        FUND_TID = table.Rows[i]["FUND_ID"].ToString(),
                        FUND_SHORTNAME = table.Rows[i]["mnumenic"].ToString()
                    });
                }

                //////////////////////////////############################################////////////////////////////////

                DataTable table1 = new DataTable();
                //using (SqlConnection con = new SqlConnection(constr))
                //{
                //    string query = "SELECT * FROM COMPANY ";
                //    //query = query + "left join REGION_CITY ci   on ci.CITY_ID=sbb.CITY_ID ";
                //    //query = query + "left join REGION_STATE st  on st.STATE_ID=sbb.STATE_ID ";
                //    //query = query + "left join STP_BANK sb on sb.BANK_ID=sbb.BANK_ID ";

                //    using (SqlCommand cmd = new SqlCommand(query, con))
                //    {
                //        SqlDataAdapter ds = new SqlDataAdapter(cmd);
                //        ds.Fill(table1);
                //    }
                //}

                using (SqlConnection con = new SqlConnection(constr_dap))
                {
                    //string query="select RATING_ID,RATING_NAME,RATING_SHORTNAME,RATING_STATUS,RATING_ISDEFAULT from config_rating ";
                    using (SqlCommand cmd = new SqlCommand("SP_GET_FUND_DropDown", con))   //.72
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        con.Open();
                        SqlDataAdapter ds = new SqlDataAdapter(cmd);
                        ds.Fill(table1);
                        con.Close();
                    }
                }

                List<Fund_Setup> fund_cmpidlist = new List<Fund_Setup>();
                for (int i = 0; i < table1.Rows.Count; i++)
                {
                    fund_cmpidlist.Add(new Fund_Setup()
                    {
                        FUND_COMPID = table1.Rows[i]["compid"].ToString(),
                        FUND_COMPName = table1.Rows[i]["compname"].ToString()
                    });
                }

                ////////////////////////////////////////######################################///////////////////////////////

                // FUND DISPLAY IN GRID VIEW 

                constr = ConfigurationManager.ConnectionStrings["DAP_NEW"].ConnectionString;

                DataTable table5 = new DataTable();
                //string constr = ConfigurationManager.ConnectionStrings["DAP_NEW"].ConnectionString;
                using (SqlConnection con = new SqlConnection(constr))
                {
                    //string query="select RATING_ID,RATING_NAME,RATING_SHORTNAME,RATING_STATUS,RATING_ISDEFAULT from config_rating ";
                    using (SqlCommand cmd = new SqlCommand("SP_GET_FUND", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        con.Open();
                        SqlDataAdapter ds = new SqlDataAdapter(cmd);
                        ds.Fill(table5);
                        con.Close();
                    }
                }
                List<Fund_Setup> fund_setup_list = new List<Fund_Setup>();
                for (int i = 0; i < table5.Rows.Count; i++)
                {
                    //  bankname="Anubhav",shortname="Chaudhary",rating="AA+"

                    fund_setup_list.Add(new Fund_Setup()
                   {
                       FUND_ID = table5.Rows[i]["FUND_ID"].ToString(),
                       FUND_TID = table5.Rows[i]["FUND_TID"].ToString(),
                       FUND_NAME = table5.Rows[i]["FUND_NAME"].ToString(),
                       FUND_SHORTNAME = table5.Rows[i]["FUND_SHORTNAME"].ToString(),
                       FUND_STATUS = table5.Rows[i]["FUND_STATUS"].ToString(),
                       FUND_COMPID = table5.Rows[i]["FUND_COMPID"].ToString()
                   });
                }
                Fund_Setup user1 = new Fund_Setup();
                user1.Fund_setup_list = fund_setup_list;
                user1.Fund_tid_list = fund_tidlist;
                user1.Fund_compid_list = fund_cmpidlist;
                // work needs to be done for insert functionality 
                return View(user1);
            }
            else 
            {
                return RedirectToAction("login", "Home");
            
            }




        }
        [HttpPost]
        public ActionResult fund_setup(Fund_Setup model, FormCollection formcollection)
        {
            //string mystring = model.trial;

            model.FUND_COMPName = formcollection["comp_name"].ToString();


            if (model.FUND_COMPName == "-- Please select --")
            {
                ModelState.AddModelError("Fund", "Kindly provide correct fund");
            }

            if (ModelState.IsValid)
            {

                string constr1 = ConfigurationManager.ConnectionStrings["DAP_T24"].ConnectionString;
                DataTable table = new DataTable();
                using (SqlConnection con = new SqlConnection(constr1))
                {
                    string query = "SELECT * FROM FUND_fundsgroup where fundmapid='" + model.FUND_COMPID.ToString() + "'";
                    //query = query + "left join REGION_CITY ci   on ci.CITY_ID=sbb.CITY_ID ";
                    //query = query + "left join REGION_STATE st  on st.STATE_ID=sbb.STATE_ID ";
                    //query = query + "left join STP_BANK sb on sb.BANK_ID=sbb.BANK_ID ";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        SqlDataAdapter ds = new SqlDataAdapter(cmd);
                        ds.Fill(table);
                    }
                }

                for (int i = 0; i < table.Rows.Count; i++)
                {
                    model.FUND_SHORTNAME = table.Rows[i]["FUND_mnemonic"].ToString();
                    model.FUND_TID = table.Rows[i]["FUND_ID"].ToString();
                }


                //    model.FUND_SHORTNAME = formcollection["fund_name_short"].ToString();
                int s = 100;
                string user_session = this.HttpContext.Session["username_session"].ToString();
                string constr = ConfigurationManager.ConnectionStrings["DAP_NEW"].ConnectionString;
                using (SqlConnection con = new SqlConnection(constr))
                {
                    con.Open();
                    // MessageBox.Show ("Connection Open ! ");
                    // cnn.Close();
                    SqlCommand cmd = new SqlCommand("SP_INSERT_FUND", con);    // to be corrected 
                    // SqlCommand cmd1 = new SqlCommand("select top 1 * from Customer_IDs where ID_NO ='" + txtCnicNo + "'", oCnn);   
                    cmd.CommandType = CommandType.StoredProcedure;
                    //cmd.Parameters.AddWithValue("@Rating_Id", model.rating_id);
                    cmd.Parameters.AddWithValue("@sFund_TID", model.FUND_TID);
                    cmd.Parameters.AddWithValue("@sFund_Name", model.FUND_COMPName);
                    cmd.Parameters.AddWithValue("@sFund_ShortName", model.FUND_SHORTNAME);
                    cmd.Parameters.AddWithValue("@sFund_Status", model.FUND_STATUS);
                    cmd.Parameters.AddWithValue("@sFund_CompID", model.FUND_COMPID);
                    cmd.Parameters.AddWithValue("@sUser", user_session);
                    s = cmd.ExecuteNonQuery();
                    con.Close();
                }

                return RedirectToAction("fund_setup", "Home");

            }
            else 
            {

                string constr = ConfigurationManager.ConnectionStrings["DAP_T24"].ConnectionString;
                string constr_dap = ConfigurationManager.ConnectionStrings["DAP_NEW"].ConnectionString;

                DataTable table = new DataTable();
                using (SqlConnection con = new SqlConnection(constr))
                {
                    string query = "SELECT * FROM FUND ";
                    //query = query + "left join REGION_CITY ci   on ci.CITY_ID=sbb.CITY_ID ";
                    //query = query + "left join REGION_STATE st  on st.STATE_ID=sbb.STATE_ID ";
                    //query = query + "left join STP_BANK sb on sb.BANK_ID=sbb.BANK_ID ";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        SqlDataAdapter ds = new SqlDataAdapter(cmd);
                        ds.Fill(table);
                    }
                }
                List<Fund_Setup> fund_tidlist = new List<Fund_Setup>();
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    fund_tidlist.Add(new Fund_Setup()
                    {
                        FUND_TID = table.Rows[i]["FUND_ID"].ToString(),
                        FUND_SHORTNAME = table.Rows[i]["mnumenic"].ToString()
                    });
                }


                //////////////////////////////############################################////////////////////////////////

                DataTable table1 = new DataTable();
                //using (SqlConnection con = new SqlConnection(constr))
                //{
                //    string query = "SELECT * FROM COMPANY ";
                //    //query = query + "left join REGION_CITY ci   on ci.CITY_ID=sbb.CITY_ID ";
                //    //query = query + "left join REGION_STATE st  on st.STATE_ID=sbb.STATE_ID ";
                //    //query = query + "left join STP_BANK sb on sb.BANK_ID=sbb.BANK_ID ";

                //    using (SqlCommand cmd = new SqlCommand(query, con))
                //    {
                //        SqlDataAdapter ds = new SqlDataAdapter(cmd);
                //        ds.Fill(table1);
                //    }
                //}

                using (SqlConnection con = new SqlConnection(constr_dap))
                {
                    //string query="select RATING_ID,RATING_NAME,RATING_SHORTNAME,RATING_STATUS,RATING_ISDEFAULT from config_rating ";
                    using (SqlCommand cmd = new SqlCommand("SP_GET_FUND_DropDown", con))   //.72
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        con.Open();
                        SqlDataAdapter ds = new SqlDataAdapter(cmd);
                        ds.Fill(table1);
                        con.Close();
                    }
                }

                List<Fund_Setup> fund_cmpidlist = new List<Fund_Setup>();
                for (int i = 0; i < table1.Rows.Count; i++)
                {
                    fund_cmpidlist.Add(new Fund_Setup()
                    {
                        FUND_COMPID = table1.Rows[i]["compid"].ToString(),
                        FUND_COMPName = table1.Rows[i]["compname"].ToString()

                    });
                }


                ////////////////////////////////////////######################################///////////////////////////////

                // FUND DISPLAY IN GRID VIEW 

                constr = ConfigurationManager.ConnectionStrings["DAP_NEW"].ConnectionString;

                DataTable table5 = new DataTable();
                //string constr = ConfigurationManager.ConnectionStrings["DAP_NEW"].ConnectionString;
                using (SqlConnection con = new SqlConnection(constr))
                {
                    //string query="select RATING_ID,RATING_NAME,RATING_SHORTNAME,RATING_STATUS,RATING_ISDEFAULT from config_rating ";
                    using (SqlCommand cmd = new SqlCommand("SP_GET_FUND", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        con.Open();
                        SqlDataAdapter ds = new SqlDataAdapter(cmd);
                        ds.Fill(table5);
                        con.Close();
                    }
                }

                List<Fund_Setup> fund_setup_list = new List<Fund_Setup>();

                for (int i = 0; i < table5.Rows.Count; i++)
                {
                    //  bankname="Anubhav",shortname="Chaudhary",rating="AA+"

                    fund_setup_list.Add(new Fund_Setup()
                    {
                        FUND_ID = table5.Rows[i]["FUND_ID"].ToString(),
                        FUND_TID = table5.Rows[i]["FUND_TID"].ToString(),
                        FUND_NAME = table5.Rows[i]["FUND_NAME"].ToString(),
                        FUND_SHORTNAME = table5.Rows[i]["FUND_SHORTNAME"].ToString(),
                        FUND_STATUS = table5.Rows[i]["FUND_STATUS"].ToString(),
                        FUND_COMPID = table5.Rows[i]["FUND_COMPID"].ToString()
                    });
                }
                Fund_Setup user1 = new Fund_Setup();
                user1.Fund_setup_list = fund_setup_list;
                user1.Fund_tid_list = fund_tidlist;
                user1.Fund_compid_list = fund_cmpidlist;
                // work needs to be done for insert functionality 
                return View(user1);
            
            
            
            }





            //return View();
        }
        public JsonResult edit_fund_setup(Fund_Setup model, FormCollection formcollection)            // changs needs to be done here 
        {
            int s = 100;
            string user_session = this.HttpContext.Session["username_session"].ToString();
            //string accounttypeid = model.ACCTYPE_ID.ToString();
            //string ratingisdefault = model.ACCTYPE_ISDEFAULT.ToString();
            //if (ratingisdefault == "TRUE")
            //{
            //    ratingisdefault = "1";
            //}
            //if (ratingisdefault == "FALSE")
            //{
            //    ratingisdefault = "0";
            //}

            //string a = formcollection["fund_shrt_name"].ToString();
            //string b = formcollection["fund_comp_name"].ToString();

            //model.FUND_SHORTNAME = formcollection["fund_name_short_edit"].ToString();
            //model.FUND_NAME= formcollection["fund_name_edit"].ToString();

            string constr = ConfigurationManager.ConnectionStrings["DAP_NEW"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                con.Open();
                // MessageBox.Show ("Connection Open ! ");
                // cnn.Close();
                SqlCommand cmd = new SqlCommand("SP_UPDATE_FUND", con);    // to be corrected 
                // SqlCommand cmd1 = new SqlCommand("select top 1 * from Customer_IDs where ID_NO ='" + txtCnicNo + "'", oCnn);   
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@iFund_Id", model.FUND_ID);
                //cmd.Parameters.AddWithValue("@sFund_Name", model.FUND_NAME);
                //cmd.Parameters.AddWithValue("@sFund_TID", model.FUND_TID);
                //cmd.Parameters.AddWithValue("@sFund_ShortName", model.FUND_SHORTNAME);
                //cmd.Parameters.AddWithValue("@sFund_CompID", model.FUND_COMPID);
                cmd.Parameters.AddWithValue("@sFund_Status", model.FUND_STATUS);
                //cmd.Parameters.AddWithValue("@BBR_STATUS", model.BBR_STATUS);
                //  cmd.Parameters.AddWithValue("", model.BANK_STATUS);        
                cmd.Parameters.AddWithValue("@sMUser", user_session);
                s = cmd.ExecuteNonQuery();
                con.Close();
            }
            string message1 = "Successful";
            string message2 = "unsuccessful";
            if (s == 0)
            {
                return Json(message2, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(message1, JsonRequestBehavior.AllowGet);
            }
            //return View();
        }
        [HttpPost]
        public ActionResult ExportData_fund_setup() 
        {
            string txtFile = string.Empty;
            DataTable table = new DataTable();
            string constr = ConfigurationManager.ConnectionStrings["DAP_NEW"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                //string query="select RATING_ID,RATING_NAME,RATING_SHORTNAME,RATING_STATUS,RATING_ISDEFAULT from config_rating ";
                using (SqlCommand cmd = new SqlCommand("SP_GET_FUND", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    con.Open();
                    SqlDataAdapter ds = new SqlDataAdapter(cmd);
                    ds.Fill(table);
                    con.Close();
                }
            }
            List<Fund_Setup_pdf> fund_setup_list = new List<Fund_Setup_pdf>();
            for (int i = 0; i < table.Rows.Count; i++)
            {
                //  bankname="Anubhav",shortname="Chaudhary",rating="AA+"
                fund_setup_list.Add(new Fund_Setup_pdf()
                {
                    Serial = table.Rows[i]["FUND_ID"].ToString(),
                    //FUND_TID = table.Rows[i]["FUND_TID"].ToString(),
                    Name = table.Rows[i]["FUND_NAME"].ToString(),
                   ShortName = table.Rows[i]["FUND_SHORTNAME"].ToString(),
                    Status = table.Rows[i]["FUND_STATUS"].ToString(),
                    CreatedBy = table.Rows[i]["FUND_CREATEDBY"].ToString(),
                    CreatedOn = table.Rows[i]["FUND_CREATEDON"].ToString(),
                    ModifiedBy = table.Rows[i]["FUND_MODIFIEDBY"].ToString(),
                    ModifiedOn = table.Rows[i]["FUND_MODIFIEDON"].ToString(),                    
                    //FUND_COMPID = table.Rows[i]["FUND_COMPID"].ToString()
                });
            }

            GridView gv = new GridView();
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "attachment;filename=Fund_Setup_Report.pdf");
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            StringWriter sw = new StringWriter();
            HtmlTextWriter hw = new HtmlTextWriter(sw);
            gv.DataSource = fund_setup_list;
            gv.AllowPaging = false;
            gv.DataBind();
            gv.HeaderRow.Style.Add("width", "300%");
            gv.HeaderRow.Style.Add("font-size", "7px");
            gv.Style.Add("text-decoration", "none");
            gv.Style.Add("font-family", "Arial, Helvetica, sans-serif;");
            gv.Style.Add("font-size", "7px");
            //gv.Style.Add("width", "300%");         
            gv.RenderControl(hw);
            StringReader sr = new StringReader(sw.ToString());
            Document pdfDoc = new Document(PageSize.A4.Rotate(), 30f, 30f, 30f, 30f);
            HTMLWorker htmlparser = new HTMLWorker(pdfDoc);
            PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
            pdfDoc.Open();
            htmlparser.Parse(sr);//this is the error line
            pdfDoc.Close();
            Response.Write(pdfDoc);
            Response.End();
            
          return RedirectToAction("fund_setup", "Home");
        }                
        public ActionResult accountpurpose_setup()
        {
            if (this.HttpContext.Session["username_session"] != null)
    {
            string constr = ConfigurationManager.ConnectionStrings["DAP_NEW"].ConnectionString;
            DataTable table = new DataTable();
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = "SELECT * FROM CONFIG_ACCOUNTPURPOSE ";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    SqlDataAdapter ds = new SqlDataAdapter(cmd);
                    ds.Fill(table);
                }
            }
            List<AccountPurpose_Setup> acctpurp_setup_list = new List<AccountPurpose_Setup>();
            for (int i = 0; i < table.Rows.Count; i++)
            {
                acctpurp_setup_list.Add(new AccountPurpose_Setup()
                {
                    ACCPUR_ID = table.Rows[i]["ACCPUR_ID"].ToString(),
                    ACCPUR_NAME = table.Rows[i]["ACCPUR_NAME"].ToString(),
                    ACCPUR_SHORTNAME = table.Rows[i]["ACCPUR_SHORTNAME"].ToString(),
                    ACCPUR_STATUS = table.Rows[i]["ACCPUR_STATUS"].ToString(),
                    ACCPUR_ISDEFAULT = table.Rows[i]["ACCPUR_ISDEFAULT"].ToString()
                    // ACCPUR_ID = table.Rows[i]["ACCPUR_ID"].ToString(),
                });
            }
            AccountPurpose_Setup user1 = new AccountPurpose_Setup();
            user1.accountpurpose_list = acctpurp_setup_list;
            return View(user1);
    }
            else
            {
                return RedirectToAction("login", "Home");

            }
                      
            }
        public JsonResult edit_accountpurpose(AccountPurpose_Setup model)
        {
            int s = 100;
            string user_session = this.HttpContext.Session["username_session"].ToString();
            string accounttypeid = model.ACCPUR_ID.ToString();
            string accounttypename = model.ACCPUR_NAME.ToString();
            string accountshortname = model.ACCPUR_SHORTNAME.ToString();
            string accounttypestatus = model.ACCPUR_STATUS.ToString();
            string ratingisdefault = model.ACCPUR_ISDEFAULT.ToString();
            if (ratingisdefault == "TRUE")
            {
                ratingisdefault = "1";
            }
            if (ratingisdefault == "FALSE")
            {
                ratingisdefault = "0";
            }


            string constr = ConfigurationManager.ConnectionStrings["DAP_NEW"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {

                con.Open();
                // MessageBox.Show ("Connection Open ! ");
                // cnn.Close();
                SqlCommand cmd = new SqlCommand("SP_UPDATE_CONFIG_ACCOUNTPURPOSE", con);    // to be corrected 
                // SqlCommand cmd1 = new SqlCommand("select top 1 * from Customer_IDs where ID_NO ='" + txtCnicNo + "'", oCnn);   
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ACCPUR_Id", model.ACCPUR_ID);
                cmd.Parameters.AddWithValue("@ACCPUR_Name", model.ACCPUR_NAME);
                cmd.Parameters.AddWithValue("@ACCPUR_ShortName", model.ACCPUR_SHORTNAME);
                cmd.Parameters.AddWithValue("@ACCPUR_Status", model.ACCPUR_STATUS);
                cmd.Parameters.AddWithValue("@ACCPUR_IsDefault", ratingisdefault);
                cmd.Parameters.AddWithValue("@sUser", user_session);
                s = cmd.ExecuteNonQuery();
                con.Close();
            }

            string message1 = "Successful";
            string message2 = "unsuccessful";
            if (s == 0)
            {
                return Json(message2, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(message1, JsonRequestBehavior.AllowGet);
            }



            //return View();

        }
        [HttpPost]
        public ActionResult accountpurpose_setup(AccountPurpose_Setup model)
        {

            if (string.IsNullOrEmpty(model.ACCPUR_NAME))
            {
                ModelState.AddModelError("Name", "Kindly Provide Name");
            }
            if (string.IsNullOrEmpty(model.ACCPUR_SHORTNAME))
            {
                ModelState.AddModelError("Name", "Kindly Provide Name");
            }
            if (string.IsNullOrEmpty(model.ACCPUR_STATUS))
            {
                ModelState.AddModelError("Name", "Kindly Provide Name");
            }
            if (ModelState.IsValid)
            {

                int s = 100;
                string user_session = this.HttpContext.Session["username_session"].ToString();
                // string ratingid = model.rating_id.ToString();
                string accttypename = model.ACCPUR_NAME.ToString();
                string accttypeshortname = model.ACCPUR_SHORTNAME.ToString();
                string accttypestatus = model.ACCPUR_STATUS.ToString();
                //string ratingisdefault = model.rating_isdefault.ToString();
                bool accttypedefault = model.ACCPUR_ISDEFAULT_BOOL;

                string constr = ConfigurationManager.ConnectionStrings["DAP_NEW"].ConnectionString;
                using (SqlConnection con = new SqlConnection(constr))
                {

                    con.Open();
                    // MessageBox.Show ("Connection Open ! ");
                    // cnn.Close();
                    SqlCommand cmd = new SqlCommand("SP_INSERT_CONFIG_ACCOUNTPURPOSE", con);    // to be corrected 
                    // SqlCommand cmd1 = new SqlCommand("select top 1 * from Customer_IDs where ID_NO ='" + txtCnicNo + "'", oCnn);   
                    cmd.CommandType = CommandType.StoredProcedure;
                    //cmd.Parameters.AddWithValue("@Rating_Id", model.rating_id);
                    cmd.Parameters.AddWithValue("@ACCPUR_Name", model.ACCPUR_NAME);
                    cmd.Parameters.AddWithValue("@ACCPUR_ShortName", model.ACCPUR_SHORTNAME);
                    cmd.Parameters.AddWithValue("@ACCPUR_Status", model.ACCPUR_STATUS);
                    cmd.Parameters.AddWithValue("@ACCPUR_IsDefault", model.ACCPUR_ISDEFAULT_BOOL);
                    cmd.Parameters.AddWithValue("@sUser", user_session);
                    s = cmd.ExecuteNonQuery();
                    con.Close();
                }
                return RedirectToAction("accountpurpose_setup", "Home");
            }
            else
            {
                string constr = ConfigurationManager.ConnectionStrings["DAP_NEW"].ConnectionString;
                DataTable table = new DataTable();
                using (SqlConnection con = new SqlConnection(constr))
                {
                    string query = "SELECT * FROM CONFIG_ACCOUNTPURPOSE ";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        SqlDataAdapter ds = new SqlDataAdapter(cmd);
                        ds.Fill(table);
                    }
                }
                List<AccountPurpose_Setup> acctpurp_setup_list = new List<AccountPurpose_Setup>();
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    acctpurp_setup_list.Add(new AccountPurpose_Setup()
                    {
                        ACCPUR_ID = table.Rows[i]["ACCPUR_ID"].ToString(),
                        ACCPUR_NAME = table.Rows[i]["ACCPUR_NAME"].ToString(),
                        ACCPUR_SHORTNAME = table.Rows[i]["ACCPUR_SHORTNAME"].ToString(),
                        ACCPUR_STATUS = table.Rows[i]["ACCPUR_STATUS"].ToString(),
                        ACCPUR_ISDEFAULT = table.Rows[i]["ACCPUR_ISDEFAULT"].ToString()
                        // ACCPUR_ID = table.Rows[i]["ACCPUR_ID"].ToString(),
                    });
                }
                AccountPurpose_Setup user1 = new AccountPurpose_Setup();
                user1.accountpurpose_list = acctpurp_setup_list;
                return View(user1);
            }

        }
        [HttpPost]
        public ActionResult ExportData_accountpurpose()
        {
            string constr = ConfigurationManager.ConnectionStrings["DAP_NEW"].ConnectionString;
            DataTable table = new DataTable();
            using (SqlConnection con = new SqlConnection(constr))
            {
string query="SELECT ACCPUR_ID,ACCPUR_NAME,ACCPUR_SHORTNAME,ACCPUR_STATUS,ACCPUR_ISDEFAULT,ACCPUR_CREATEDBY,ACCPUR_CREATEDON,ACCPUR_MODIFIEDBY,convert (varchar (10),[ACCPUR_MODIFIEDON],126) ACCPUR_MODIFIEDON FROM CONFIG_ACCOUNTPURPOSE";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    SqlDataAdapter ds = new SqlDataAdapter(cmd);
                    ds.Fill(table);
                }
            }
            List<AccountPurpose_Setup_pdf> acctpurp_setup_list = new List<AccountPurpose_Setup_pdf>();
            for (int i = 0; i < table.Rows.Count; i++)
            {
                acctpurp_setup_list.Add(new AccountPurpose_Setup_pdf()
                {
                    Serial = table.Rows[i]["ACCPUR_ID"].ToString(),
                    Name = table.Rows[i]["ACCPUR_NAME"].ToString(),
                    Shortname = table.Rows[i]["ACCPUR_SHORTNAME"].ToString(),
                    Status = table.Rows[i]["ACCPUR_STATUS"].ToString(),
                    IsDefault = table.Rows[i]["ACCPUR_ISDEFAULT"].ToString(),
                    CreatedBy= table.Rows[i]["ACCPUR_CREATEDBY"].ToString(),
                    CreatedOn = table.Rows[i]["ACCPUR_CREATEDON"].ToString(),
                    ModifiedBy = table.Rows[i]["ACCPUR_MODIFIEDBY"].ToString(),
                    ModifiedOn = table.Rows[i]["ACCPUR_MODIFIEDON"].ToString(),
                    // ACCPUR_ID = table.Rows[i]["ACCPUR_ID"].ToString(),
                });
            }
            GridView gv = new GridView();
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition","attachment;filename=Acctpurp_Setup_Report.pdf");
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            StringWriter sw = new StringWriter();
            HtmlTextWriter hw = new HtmlTextWriter(sw);
            gv.DataSource = acctpurp_setup_list;
            gv.AllowPaging = false;
            gv.DataBind();
          
            gv.HeaderRow.Style.Add("width", "300%");
            gv.HeaderRow.Style.Add("font-size", "7px");
            gv.Style.Add("text-decoration", "none");
            gv.Style.Add("font-family", "Arial, Helvetica, sans-serif;");
            gv.Style.Add("font-size", "7px");
            //gv.Style.Add("width", "300%");         
            gv.RenderControl(hw);
            StringReader sr = new StringReader(sw.ToString());
            Document pdfDoc = new Document(PageSize.A4.Rotate(), 30f, 30f, 30f, 30f);
            HTMLWorker htmlparser = new HTMLWorker(pdfDoc);
            PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
            pdfDoc.Open();
            htmlparser.Parse(sr);//this is the error line
            pdfDoc.Close();
            Response.Write(pdfDoc);
            Response.End();
            return RedirectToAction("accountpurpose_setup", "Home");
            // return RedirectToAction("Rating", "Home");
        
        }
        public ActionResult fundbankaccount_setup()
        {
          if (this.HttpContext.Session["username_session"] != null)
          {

              ViewBag.Message = null;


            string constr = ConfigurationManager.ConnectionStrings["DAP_NEW"].ConnectionString;
            DataTable table = new DataTable();
            using (SqlConnection con = new SqlConnection(constr))
            {
                //string query = "SELECT * FROM CONFIG_ACCOUNTPURPOSE ";
                using (SqlCommand cmd = new SqlCommand("SP_GET_STP_FUNDBANKACCOUNT", con))
                {
                    SqlDataAdapter ds = new SqlDataAdapter(cmd);
                    ds.Fill(table);
                }
            }
            List<Fundbankaccount_Setup> fundbankacct_setup_list = new List<Fundbankaccount_Setup>();
            for (int i = 0; i < table.Rows.Count; i++)
            {
                fundbankacct_setup_list.Add(new Fundbankaccount_Setup()
                {
                    FBACC_ID = table.Rows[i]["FBACC_ID"].ToString(),
                    FUND_ID = table.Rows[i]["FUND_ID"].ToString(),
                    FUND_NAME = table.Rows[i]["FUND_NAME"].ToString(),
                    BANK_ID = table.Rows[i]["BANK_ID"].ToString(),
                    BANK_NAME = table.Rows[i]["BANK_NAME"].ToString(),
                    BBR_ID = table.Rows[i]["BBR_ID"].ToString(),
                    BBR_NAME = table.Rows[i]["BBR_NAME"].ToString(),
                    ACCTYPE_ID = table.Rows[i]["ACCTYPE_ID"].ToString(),
                    ACCTYPE_NAME = table.Rows[i]["ACCTYPE_NAME"].ToString(),
                    PBAS_ID = table.Rows[i]["PBAS_ID"].ToString(),
                    PBAS_NAME = table.Rows[i]["PBAS_NAME"].ToString(),
                    PFRQ_ID = table.Rows[i]["PFRQ_ID"].ToString(),
                    PFRQ_NAME = table.Rows[i]["PFRQ_NAME"].ToString(),
                    FBACC__ISPOOLED = table.Rows[i]["FBACC_ISPOOLED"].ToString(),
                    FBACC_NUMBER = table.Rows[i]["FBACC_NUMBER"].ToString(),
                    FBACC_TCREDITACCOUNT = table.Rows[i]["FBACC_TCREDITACCOUNT"].ToString(),
                    FBACC_TDEBITACCOUNT = table.Rows[i]["FBACC_TDEBITACCOUNT"].ToString(),
                    FBACC_ISCLOSED = table.Rows[i]["FBACC_ISCLOSED"].ToString(),
                    FBACC_OPENINGDATE = table.Rows[i]["FBACC_OPENINGDATE"].ToString(),
                    FBACC_CLOSINGDATE = table.Rows[i]["FBACC_CLOSINGDATE"].ToString(),
                    FBACC_STATUS = table.Rows[i]["FBACC_STATUS"].ToString(),
                    FBACC_TITLE = table.Rows[i]["FBACC_TITLE"].ToString(),
                    ACCPUR_ID = table.Rows[i]["ACCPUR_ID"].ToString(),
                    ACCPUR_NAME = table.Rows[i]["ACCPUR_NAME"].ToString(),
                    FBACC_COMPID = table.Rows[i]["FBACC_COMPID"].ToString(),
                    SLAB_ID = table.Rows[i]["Slab_id"].ToString(),
                    SLAB_NAME = table.Rows[i]["SLAB_NAME"].ToString()
                    
                    // ACCPUR_ID = table.Rows[i]["ACCPUR_ID"].ToString(),
                });
            }

            //////////////////////////////////############################////////////////////////

            //string constr = ConfigurationManager.ConnectionStrings["DAP_NEW"].ConnectionString;
            DataTable table1 = new DataTable();
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = "select * from STP_FUND where fund_status='Active'";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    SqlDataAdapter ds = new SqlDataAdapter(cmd);
                    ds.Fill(table1);
                }
            }
            List<Fundbankaccount_Setup> fund_list = new List<Fundbankaccount_Setup>();

            for (int i = 0; i < table1.Rows.Count; i++)
            {
                //  bankname="Anubhav",shortname="Chaudhary",rating="AA+"

                fund_list.Add(new Fundbankaccount_Setup()
                {
                    FUND_ID = table1.Rows[i]["FUND_ID"].ToString(),
                    FUND_NAME = table1.Rows[i]["FUND_NAME"].ToString(),
                    FBACC_COMPID = table1.Rows[i]["FUND_COMPID"].ToString()
                });
            }

            //////////////////////////////////############################////////////////////////

            DataTable table2 = new DataTable();
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = "select * from STP_BANK";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    SqlDataAdapter ds = new SqlDataAdapter(cmd);
                    ds.Fill(table2);
                }
            }
            List<Fundbankaccount_Setup> bank_list = new List<Fundbankaccount_Setup>();

            for (int i = 0; i < table2.Rows.Count; i++)
            {
                //  bankname="Anubhav",shortname="Chaudhary",rating="AA+"

                bank_list.Add(new Fundbankaccount_Setup()
                {
                    BANK_NAME = table2.Rows[i]["BANK_NAME"].ToString(),
                    BANK_ID = table2.Rows[i]["BANK_ID"].ToString()
                });
            }

            //////////////////////////////////############################////////////////////////

            DataTable table3 = new DataTable();
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = "SELECT * FROM STP_BANKBRANCH";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    SqlDataAdapter ds = new SqlDataAdapter(cmd);
                    ds.Fill(table3);
                }
            }
            List<Fundbankaccount_Setup> branch_list = new List<Fundbankaccount_Setup>();

            for (int i = 0; i < table3.Rows.Count; i++)
            {
                //  bankname="Anubhav",shortname="Chaudhary",rating="AA+"

                branch_list.Add(new Fundbankaccount_Setup()
                {
                    BBR_ID = table3.Rows[i]["BBR_ID"].ToString(),
                    BBR_NAME = table3.Rows[i]["BBR_NAME"].ToString()
                });
            }

            //////////////////////////////////############################////////////////////////

            DataTable table4 = new DataTable();
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = "SELECT * FROM CONFIG_ACCOUNTTYPE";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    SqlDataAdapter ds = new SqlDataAdapter(cmd);
                    ds.Fill(table4);
                }
            }
            List<Fundbankaccount_Setup> acctype_list = new List<Fundbankaccount_Setup>();

            for (int i = 0; i < table4.Rows.Count; i++)
            {
                //  bankname="Anubhav",shortname="Chaudhary",rating="AA+"

                acctype_list.Add(new Fundbankaccount_Setup()
                {
                    ACCTYPE_ID = table4.Rows[i]["ACCTYPE_ID"].ToString(),
                    ACCTYPE_NAME = table4.Rows[i]["ACCTYPE_NAME"].ToString()
                });
            }

            //////////////////////////////////############################////////////////////////

            DataTable table5 = new DataTable();
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = "select * from CONFIG_ACCOUNTPURPOSE";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    SqlDataAdapter ds = new SqlDataAdapter(cmd);
                    ds.Fill(table5);
                }
            }
            List<Fundbankaccount_Setup> accpur_list = new List<Fundbankaccount_Setup>();

            for (int i = 0; i < table5.Rows.Count; i++)
            {
                //  bankname="Anubhav",shortname="Chaudhary",rating="AA+"

                accpur_list.Add(new Fundbankaccount_Setup()
                {
                    ACCPUR_ID = table5.Rows[i]["ACCPUR_ID"].ToString(),
                    ACCPUR_NAME = table5.Rows[i]["ACCPUR_NAME"].ToString()
                });
            }

            //////////////////////////////////############################////////////////////////

            DataTable table6 = new DataTable();
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = "select * from CONFIG_PROFITBASIS";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    SqlDataAdapter ds = new SqlDataAdapter(cmd);
                    ds.Fill(table6);
                }
            }
            List<Fundbankaccount_Setup> PBAS_list = new List<Fundbankaccount_Setup>();

            for (int i = 0; i < table6.Rows.Count; i++)
            {
                //  bankname="Anubhav",shortname="Chaudhary",rating="AA+"

                PBAS_list.Add(new Fundbankaccount_Setup()
                {
                    PBAS_ID = table6.Rows[i]["PBAS_ID"].ToString(),
                    PBAS_NAME = table6.Rows[i]["PBAS_NAME"].ToString()
                });
            }

            //////////////////////////////////############################////////////////////////

            DataTable table7 = new DataTable();
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = "select * from CONFIG_PROFITFREQUENCY";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    SqlDataAdapter ds = new SqlDataAdapter(cmd);
                    ds.Fill(table7);
                }
            }
            List<Fundbankaccount_Setup> pfrq_list = new List<Fundbankaccount_Setup>();

            for (int i = 0; i < table7.Rows.Count; i++)
            {
                //  bankname="Anubhav",shortname="Chaudhary",rating="AA+"

                pfrq_list.Add(new Fundbankaccount_Setup()
                {
                    PFRQ_ID = table7.Rows[i]["PFRQ_ID"].ToString(),
                    PFRQ_NAME = table7.Rows[i]["PFRQ_NAME"].ToString()
                });
            }

              //////////////////////////////###################///////////////////////

            DataTable table8 = new DataTable();
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = "select * from STP_BALAMOUNTSLABS";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    SqlDataAdapter ds = new SqlDataAdapter(cmd);
                    ds.Fill(table8);
                }
            }
            List<Fundbankaccount_Setup> slab_list = new List<Fundbankaccount_Setup>();

            for (int i = 0; i < table8.Rows.Count; i++)
            {
                slab_list.Add(new Fundbankaccount_Setup()
                {
                    SLAB_ID = table8.Rows[i]["SLAB_ID"].ToString(),
                    SLAB_NAME = table8.Rows[i]["SLAB_NAME"].ToString()
                });
            }


            //////////////////////////////////############################////////////////////////





            Fundbankaccount_Setup user1 = new Fundbankaccount_Setup();
            user1.fundbankaccount_setup_list = fundbankacct_setup_list;
            user1.branch_list = branch_list;
            user1.bank_list = bank_list;
            user1.acctype_list = acctype_list;
            user1.accpur_list = accpur_list;
            user1.pbas_list = PBAS_list;
            user1.pfrq_list = pfrq_list;
            user1.fund_list = fund_list;
            user1.slab_list = slab_list;
            user1.Closing_Date = DateTime.Parse("01/01/2070");
            user1.Opening_Date = DateTime.Parse("01/01/1900");
              //   this.HttpContext.Session["drp_fbacc_bank"] = "1"; 
            return View(user1);
          }
          else
          {
              return RedirectToAction("login", "Home");
          }

          }
        [HttpPost]
        public ActionResult fundbankaccount_setup(Fundbankaccount_Setup model)
        {
            string bid = model.BANK_ID.ToString();


            //string b = model.dll_bank.ToString();


            //string bid = "Fruit Name: " + formcollection["FruitName"];

            //string bnae = formcollection["FruitId"];
            
            int v = 100;
            int s = 100;
            string user_session = this.HttpContext.Session["username_session"].ToString();
            string constr = ConfigurationManager.ConnectionStrings["DAP_NEW"].ConnectionString;

            bool ISPOOLED = model.FBACC__ISPOOLED_BOOL;
            bool ISCLOSED = model.FBACC_ISCLOSED_BOOL;

            if (string.IsNullOrEmpty(model.FUND_ID))
            {
                ModelState.AddModelError("Name", "Please Enter Bank Name");
            }
            if (string.IsNullOrEmpty(model.BANK_ID))
            {
                ModelState.AddModelError("Name", "Please Enter Bank Name");
            }
            if (string.IsNullOrEmpty(model.BBR_ID))
            {
                ModelState.AddModelError("Name", "Please Enter Bank Name");
            }
            if (string.IsNullOrEmpty(model.ACCTYPE_ID))
            {
                ModelState.AddModelError("Name", "Please Enter Bank Name");
            }
            if (string.IsNullOrEmpty(model.ACCPUR_ID))
            {
                ModelState.AddModelError("Name", "Please Enter Bank Name");
            }
            if (string.IsNullOrEmpty(model.PBAS_ID))
            {
                ModelState.AddModelError("Name", "Please Enter Bank Name");
            }
            if (string.IsNullOrEmpty(model.PFRQ_ID))
            {
                ModelState.AddModelError("Name", "Please Enter Bank Name");
            }
            if (string.IsNullOrEmpty(model.FBACC_NUMBER))
            {
                ModelState.AddModelError("Name", "Please Enter Bank Name");
            }
            if (string.IsNullOrEmpty(model.FBACC_TITLE))
            {
                ModelState.AddModelError("Name", "Please Enter Bank Name");
            }
            if (string.IsNullOrEmpty(model.FBACC_TDEBITACCOUNT))
            {
               // ModelState.AddModelError("Name", "Please Enter Bank Name");
                model.FBACC_TDEBITACCOUNT = "";            
            }
            if (string.IsNullOrEmpty(model.FBACC_TCREDITACCOUNT))
            {
                //ModelState.AddModelError("Name", "Please Enter Bank Name");
                model.FBACC_TCREDITACCOUNT = "";
            }
            if (string.IsNullOrEmpty(model.FBACC_STATUS))
            {
                ModelState.AddModelError("Name", "Please Enter Bank Name");
            }
            if (string.IsNullOrEmpty(model.FBACC_COMPID))
            {
                ModelState.AddModelError("Name", "Please Enter Bank Name");  // works needs to be done 
            }
            if (string.IsNullOrEmpty(model.SLAB_ID))
            {
                ModelState.AddModelError("Name", "Kindly provide slab");
            }

            DateTime dt1 = model.Opening_Date;
            DateTime dt2 = model.Closing_Date;
            DateTime defaultdate = DateTime.Parse("01/01/0001");    //"01/01/0001"

            //if (defaultdate == dt2)
            //{
            //    if (model.FBACC_ISCLOSED_BOOL == true)
            //    {
            //        ModelState.AddModelError("Name", "kindly enter closing date");
            //    }

            //}

            if (defaultdate == dt2)
            {
                model.Closing_Date = DateTime.Parse("01/01/1900");
            }

            if (defaultdate != dt2)
            {

                if (model.Opening_Date > dt2)
                {
                    if (model.FBACC_ISCLOSED_BOOL == true)
                    {
                        ModelState.AddModelError("Range", "kindly enter correct opening date");
                    }
                }

            }//if (model.Opening_Date < dt2)
            //{

            //    model.checkdate = "T";

            //}

            //if (string.IsNullOrEmpty(model.checkdate))
            //{
            //    ModelState.AddModelError("Name", "Please Enter Bank Name");
            //}


            //if ()
            //{

            //    //int a = model.Bank_Code.Length;
            //    if (model.Opening_Date="2018-05-01")
            //    { ModelState.AddModelError("Name", "Please Enter the correct bank code"); }

            //}
            //else
            //{
            //    ModelState.AddModelError("Name", "Please Enter Bank Code");
            //}

         
            if (ModelState.IsValid)
            {
                using (SqlConnection con = new SqlConnection(constr))
                {
                    con.Open();
                    // MessageBox.Show ("Connection Open ! ");
                    // cnn.Close();
                    SqlCommand cmd = new SqlCommand("SP_INSERT_FUNDBANKACCOUNT", con);    // to be corrected 
                    // SqlCommand cmd1 = new SqlCommand("select top 1 * from Customer_IDs where ID_NO ='" + txtCnicNo + "'", oCnn);   
                    cmd.CommandType = CommandType.StoredProcedure;
                    //cmd.Parameters.AddWithValue("@Rating_Id", model.rating_id);
                    cmd.Parameters.AddWithValue("@FUND_ID", Int32.Parse(model.FUND_ID));
                    cmd.Parameters.AddWithValue("@BANK_ID", Int32.Parse(model.BANK_ID));
                    cmd.Parameters.AddWithValue("@BBR_ID", Int32.Parse(model.BBR_ID));
                    cmd.Parameters.AddWithValue("@ACCTYPE_ID", Int32.Parse(model.ACCTYPE_ID));
                    cmd.Parameters.AddWithValue("@ACCPUR_ID", Int32.Parse(model.ACCPUR_ID));
                    cmd.Parameters.AddWithValue("@PBAS_ID", Int32.Parse(model.PBAS_ID));
                    cmd.Parameters.AddWithValue("@PFRQ_ID", Int32.Parse(model.PFRQ_ID));
                    cmd.Parameters.AddWithValue("@FBACC_ISPOOLED", ISPOOLED);
                    cmd.Parameters.AddWithValue("@FBACC_NUMBER", model.FBACC_NUMBER);
                    cmd.Parameters.AddWithValue("@FBACC_TITLE", model.FBACC_TITLE);
                    cmd.Parameters.AddWithValue("@FBACC_OPENINGDATE", model.Opening_Date);
                    cmd.Parameters.AddWithValue("@FBACC_ISCLOSED", ISCLOSED);
                    cmd.Parameters.AddWithValue("@FBACC_CLOSINGDATE", model.Closing_Date);
                    cmd.Parameters.AddWithValue("@FBACC_TDEBITACCOUNT", model.FBACC_TDEBITACCOUNT);
                    cmd.Parameters.AddWithValue("@FBACC_TCREDITACCOUNT", model.FBACC_TCREDITACCOUNT);
                    cmd.Parameters.AddWithValue("@FBACC_STATUS", model.FBACC_STATUS);
                    cmd.Parameters.AddWithValue("@FBACC_COMPID", model.FBACC_COMPID);
                    cmd.Parameters.AddWithValue("@SLAB_ID", Int32.Parse(model.SLAB_ID));
                    
                    cmd.Parameters.AddWithValue("@sUser", user_session);
                    
                    s = cmd.ExecuteNonQuery();
                    con.Close();
               //////////////////////////////////////////////////////////////////////////////////////////////
                    DataTable table32 = new DataTable();
                    using (SqlConnection con2 = new SqlConnection(constr))
                    {
                        string query = "SELECT max(fbacc_id) as fbaccid  FROM STP_FUNDBANKACCOUNT ";
                        using (SqlCommand cmd2 = new SqlCommand(query, con2))
                        {
                            SqlDataAdapter ds = new SqlDataAdapter(cmd2);
                            ds.Fill(table32);
                        }
                    }
                    //  List<Fundbankaccount_Setup> fbclist = new List<Fundbankaccount_Setup>();

                    model.FBACCID_tran_detail = table32.Rows[0][0].ToString();                      
                     
             //////////////////////////////////////////////////////////////////////////////////

                    //con.Open();
                    //// MessageBox.Show ("Connection Open ! ");
                    //// cnn.Close();
                    //SqlCommand cmd1 = new SqlCommand("SP_INSERT_Tran_Detail_MSTR", con);    // to be corrected 
                    //// SqlCommand cmd1 = new SqlCommand("select top 1 * from Customer_IDs where ID_NO ='" + txtCnicNo + "'", oCnn);   
                    //cmd1.CommandType = CommandType.StoredProcedure;
                    ////cmd.Parameters.AddWithValue("@Rating_Id", model.rating_id);
                    //cmd1.Parameters.AddWithValue("@TXNMSTR_VALDATE", model.Opening_Date);
                    //cmd1.Parameters.AddWithValue("@TXNMSTR_POSTDATE", "2000-01-01");
                    //cmd1.Parameters.AddWithValue("@FBACC_ID", Int32.Parse(model.FBACCID_tran_detail));
                    //cmd1.Parameters.AddWithValue("@PTRATE_PERCENT", 1);
                    //cmd1.Parameters.AddWithValue("@TXND_AMOUNT", 1);
                    //cmd1.Parameters.AddWithValue("@TXND_ProfitAMOUNT", 1);
                    //cmd1.Parameters.AddWithValue("@sUser", user_session);

                    //v = cmd1.ExecuteNonQuery();
                    //con.Close();

                
                }
                return RedirectToAction("fundbankaccount_setup", "Home");
            }

            else
            {

                //   string constr = ConfigurationManager.ConnectionStrings["DAP_NEW"].ConnectionString;
                DataTable table = new DataTable();
                using (SqlConnection con = new SqlConnection(constr))
                {
                    //string query = "SELECT * FROM CONFIG_ACCOUNTPURPOSE ";
                    using (SqlCommand cmd = new SqlCommand("SP_GET_STP_FUNDBANKACCOUNT", con))
                    {
                        SqlDataAdapter ds = new SqlDataAdapter(cmd);
                        ds.Fill(table);
                    }
                }
                List<Fundbankaccount_Setup> fundbankacct_setup_list = new List<Fundbankaccount_Setup>();
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    fundbankacct_setup_list.Add(new Fundbankaccount_Setup()
                    {
                        FBACC_ID = table.Rows[i]["FBACC_ID"].ToString(),
                        FUND_ID = table.Rows[i]["FUND_ID"].ToString(),
                        FUND_NAME = table.Rows[i]["FUND_NAME"].ToString(),
                        BANK_ID = table.Rows[i]["BANK_ID"].ToString(),
                        BANK_NAME = table.Rows[i]["BANK_NAME"].ToString(),
                        BBR_ID = table.Rows[i]["BBR_ID"].ToString(),
                        BBR_NAME = table.Rows[i]["BBR_NAME"].ToString(),
                        ACCTYPE_ID = table.Rows[i]["ACCTYPE_ID"].ToString(),
                        ACCTYPE_NAME = table.Rows[i]["ACCTYPE_NAME"].ToString(),
                        PBAS_ID = table.Rows[i]["PBAS_ID"].ToString(),
                        PBAS_NAME = table.Rows[i]["PBAS_NAME"].ToString(),
                        PFRQ_ID = table.Rows[i]["PFRQ_ID"].ToString(),
                        PFRQ_NAME = table.Rows[i]["PFRQ_NAME"].ToString(),
                        FBACC__ISPOOLED = table.Rows[i]["FBACC_ISPOOLED"].ToString(),
                        FBACC_NUMBER = table.Rows[i]["FBACC_NUMBER"].ToString(),
                        FBACC_TCREDITACCOUNT = table.Rows[i]["FBACC_TCREDITACCOUNT"].ToString(),
                        FBACC_TDEBITACCOUNT = table.Rows[i]["FBACC_TDEBITACCOUNT"].ToString(),
                        FBACC_ISCLOSED = table.Rows[i]["FBACC_ISCLOSED"].ToString(),
                        FBACC_OPENINGDATE = table.Rows[i]["FBACC_OPENINGDATE"].ToString(),
                        FBACC_CLOSINGDATE = table.Rows[i]["FBACC_CLOSINGDATE"].ToString(),
                        FBACC_STATUS = table.Rows[i]["FBACC_STATUS"].ToString(),
                        FBACC_TITLE = table.Rows[i]["FBACC_TITLE"].ToString(),
                        ACCPUR_ID = table.Rows[i]["ACCPUR_ID"].ToString(),
                        ACCPUR_NAME = table.Rows[i]["ACCPUR_NAME"].ToString(),
                        FBACC_COMPID = table.Rows[i]["FBACC_COMPID"].ToString()
                        // ACCPUR_ID = table.Rows[i]["ACCPUR_ID"].ToString(),
                    });
                }

                //////////////////////////////////############################////////////////////////

                List<Fundbankaccount_Setup> fund_list = new List<Fundbankaccount_Setup>();

                DataTable table1_dll_fund= new DataTable();
                using (SqlConnection con = new SqlConnection(constr))
                {
                    string query = "select * from STP_FUND where fund_status='Active' and fund_id='" + model.FUND_ID + "'";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        SqlDataAdapter ds = new SqlDataAdapter(cmd);
                        ds.Fill(table1_dll_fund);
                    }
                }

                for (int i = 0; i < table1_dll_fund.Rows.Count; i++)
                {
                    //  bankname="Anubhav",shortname="Chaudhary",rating="AA+"

                    fund_list.Add(new Fundbankaccount_Setup()
                    {
                        FUND_ID = table1_dll_fund.Rows[i]["FUND_ID"].ToString(),
                        FUND_NAME = table1_dll_fund.Rows[i]["FUND_NAME"].ToString(),
                        FBACC_COMPID = table1_dll_fund.Rows[i]["FUND_COMPID"].ToString()
                    });
                }
                //string constr = ConfigurationManager.ConnectionStrings["DAP_NEW"].ConnectionString;
                DataTable table1 = new DataTable();
                using (SqlConnection con = new SqlConnection(constr))
                {
                    string query = "select * from STP_FUND where fund_status='Active' and fund_id!='" + model.FUND_ID + "'";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        SqlDataAdapter ds = new SqlDataAdapter(cmd);
                        ds.Fill(table1);
                    }
                }
               

                for (int i = 0; i < table1.Rows.Count; i++)
                {
                    //  bankname="Anubhav",shortname="Chaudhary",rating="AA+"

                    fund_list.Add(new Fundbankaccount_Setup()
                    {
                        FUND_ID = table1.Rows[i]["FUND_ID"].ToString(),
                        FUND_NAME = table1.Rows[i]["FUND_NAME"].ToString(),
                        FBACC_COMPID = table1.Rows[i]["FUND_COMPID"].ToString()
                    });
                }

                //////////////////////////////////############################////////////////////////

                DataTable table2_dll_bank = new DataTable();
                using (SqlConnection con = new SqlConnection(constr))
                {
                    string query = "select * from STP_BANK"+" where bank_id='"+model.BANK_ID+"'";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        SqlDataAdapter ds = new SqlDataAdapter(cmd);
                        ds.Fill(table2_dll_bank);
                    }
                }
                List<Fundbankaccount_Setup> bank_list = new List<Fundbankaccount_Setup>();                
                for (int i = 0; i < table2_dll_bank.Rows.Count; i++)
                {
                    //  bankname="Anubhav",shortname="Chaudhary",rating="AA+"

                    bank_list.Add(new Fundbankaccount_Setup()
                    {
                        BANK_NAME = table2_dll_bank.Rows[i]["BANK_NAME"].ToString(),
                        BANK_ID = table2_dll_bank.Rows[i]["BANK_ID"].ToString()
                    });
                }

                DataTable table2 = new DataTable();
                using (SqlConnection con = new SqlConnection(constr))
                {
                    string query = "select * from STP_BANK";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        SqlDataAdapter ds = new SqlDataAdapter(cmd);
                        ds.Fill(table2);
                    }
                }
             
                for (int i = 0; i < table2.Rows.Count; i++)
                {
                    //  bankname="Anubhav",shortname="Chaudhary",rating="AA+"

                    bank_list.Add(new Fundbankaccount_Setup()
                    {
                        BANK_NAME = table2.Rows[i]["BANK_NAME"].ToString(),
                        BANK_ID = table2.Rows[i]["BANK_ID"].ToString()
                    });
                }

                //////////////////////////////////############################////////////////////////

                DataTable table3 = new DataTable();
                using (SqlConnection con = new SqlConnection(constr))
                {
                    string query = "SELECT * FROM STP_BANKBRANCH "+" where BANK_ID='"+bid+"'";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        SqlDataAdapter ds = new SqlDataAdapter(cmd);
                        ds.Fill(table3);
                    }
                }
                List<Fundbankaccount_Setup> branch_list = new List<Fundbankaccount_Setup>();

                for (int i = 0; i < table3.Rows.Count; i++)
                {
                    //  bankname="Anubhav",shortname="Chaudhary",rating="AA+"

                    branch_list.Add(new Fundbankaccount_Setup()
                    {
                        BBR_ID = table3.Rows[i]["BBR_ID"].ToString(),
                        BBR_NAME = table3.Rows[i]["BBR_NAME"].ToString()
                    });
                }

                //////////////////////////////////############################////////////////////////

                DataTable table4 = new DataTable();
                using (SqlConnection con = new SqlConnection(constr))
                {
                    string query = "SELECT * FROM CONFIG_ACCOUNTTYPE";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        SqlDataAdapter ds = new SqlDataAdapter(cmd);
                        ds.Fill(table4);
                    }
                }
                List<Fundbankaccount_Setup> acctype_list = new List<Fundbankaccount_Setup>();

                for (int i = 0; i < table4.Rows.Count; i++)
                {
                    //  bankname="Anubhav",shortname="Chaudhary",rating="AA+"

                    acctype_list.Add(new Fundbankaccount_Setup()
                    {
                        ACCTYPE_ID = table4.Rows[i]["ACCTYPE_ID"].ToString(),
                        ACCTYPE_NAME = table4.Rows[i]["ACCTYPE_NAME"].ToString()
                    });
                }

                //////////////////////////////////############################////////////////////////

                DataTable table5 = new DataTable();
                using (SqlConnection con = new SqlConnection(constr))
                {
                    string query = "select * from CONFIG_ACCOUNTPURPOSE";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        SqlDataAdapter ds = new SqlDataAdapter(cmd);
                        ds.Fill(table5);
                    }
                }
                List<Fundbankaccount_Setup> accpur_list = new List<Fundbankaccount_Setup>();

                for (int i = 0; i < table5.Rows.Count; i++)
                {
                    //  bankname="Anubhav",shortname="Chaudhary",rating="AA+"

                    accpur_list.Add(new Fundbankaccount_Setup()
                    {
                        ACCPUR_ID = table5.Rows[i]["ACCPUR_ID"].ToString(),
                        ACCPUR_NAME = table5.Rows[i]["ACCPUR_NAME"].ToString()
                    });
                }

                //////////////////////////////////############################////////////////////////

                DataTable table6 = new DataTable();
                using (SqlConnection con = new SqlConnection(constr))
                {
                    string query = "select * from CONFIG_PROFITBASIS";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        SqlDataAdapter ds = new SqlDataAdapter(cmd);
                        ds.Fill(table6);
                    }
                }
                List<Fundbankaccount_Setup> PBAS_list = new List<Fundbankaccount_Setup>();

                for (int i = 0; i < table6.Rows.Count; i++)
                {
                    //  bankname="Anubhav",shortname="Chaudhary",rating="AA+"

                    PBAS_list.Add(new Fundbankaccount_Setup()
                    {
                        PBAS_ID = table6.Rows[i]["PBAS_ID"].ToString(),
                        PBAS_NAME = table6.Rows[i]["PBAS_NAME"].ToString()
                    });
                }

                //////////////////////////////////############################////////////////////////

                DataTable table7 = new DataTable();
                using (SqlConnection con = new SqlConnection(constr))
                {
                    string query = "select * from CONFIG_PROFITFREQUENCY";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        SqlDataAdapter ds = new SqlDataAdapter(cmd);
                        ds.Fill(table7);
                    }
                }
                List<Fundbankaccount_Setup> pfrq_list = new List<Fundbankaccount_Setup>();

                for (int i = 0; i < table7.Rows.Count; i++)
                {
                    //  bankname="Anubhav",shortname="Chaudhary",rating="AA+"

                    pfrq_list.Add(new Fundbankaccount_Setup()
                    {
                        PFRQ_ID = table7.Rows[i]["PFRQ_ID"].ToString(),
                        PFRQ_NAME = table7.Rows[i]["PFRQ_NAME"].ToString()
                    });
                }

                DataTable table8 = new DataTable();
                using (SqlConnection con = new SqlConnection(constr))
                {
                    string query = "select * from STP_BALAMOUNTSLABS";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        SqlDataAdapter ds = new SqlDataAdapter(cmd);
                        ds.Fill(table8);
                    }
                }
                List<Fundbankaccount_Setup> slab_list = new List<Fundbankaccount_Setup>();

                for (int i = 0; i < table8.Rows.Count; i++)
                {
                    //  bankname="Anubhav",shortname="Chaudhary",rating="AA+"

                    slab_list.Add(new Fundbankaccount_Setup()
                    {
                        SLAB_ID= table8.Rows[i]["SLAB_ID"].ToString(),
                        SLAB_NAME = table8.Rows[i]["SLAB_NAME"].ToString()
                    });
                }

                //////////////////////////////////############################////////////////////////


                Fundbankaccount_Setup user1 = new Fundbankaccount_Setup();
                user1.fundbankaccount_setup_list = fundbankacct_setup_list;
                user1.branch_list = branch_list;
                user1.bank_list = bank_list;
                user1.acctype_list = acctype_list;
                user1.accpur_list = accpur_list;
                user1.pbas_list = PBAS_list;
                user1.pfrq_list = pfrq_list;
                user1.fund_list = fund_list;
                user1.slab_list = slab_list;

              


                return View(user1);

            }


            // return View();
        }
        public JsonResult edit_fundbankaccount(Fundbankaccount_Setup model)
        {

            //DateTime dt1 = model.Opening_Date;
            //DateTime dt2 = model.Closing_Date;
            model.Opening_Date = DateTime.Parse(model.FBACC_OPENINGDATE);    //"01/01/0001"
            model.Closing_Date = DateTime.Parse(model.FBACC_CLOSINGDATE);    //"01/01/0001"
            string ISCLOSED = model.FBACC_ISCLOSED;
            if (ISCLOSED == "TRUE")
            {
                ISCLOSED = "1";
            }
            if (ISCLOSED == "FALSE")
            {
                ISCLOSED = "0";
            }
            string ISPOOLED = model.FBACC__ISPOOLED;
            if (ISPOOLED == "TRUE")
            {
                ISPOOLED = "1";
            }
            if (ISPOOLED == "FALSE")
            {
                ISPOOLED = "0";
            }
            if (string.IsNullOrEmpty(model.FBACC_TDEBITACCOUNT))
            {
                // ModelState.AddModelError("Name", "Please Enter Bank Name");
                model.FBACC_TDEBITACCOUNT = "";
            }
            if (string.IsNullOrEmpty(model.FBACC_TCREDITACCOUNT))
            {
                //ModelState.AddModelError("Name", "Please Enter Bank Name");
                model.FBACC_TCREDITACCOUNT = "";
            }




            int s = 100;
            string user_session = this.HttpContext.Session["username_session"].ToString();
            //string accounttypeid = model.ACCTYPE_ID.ToString();
            //string accounttypename = model.ACCTYPE_NAME.ToString();
            //string accountshortname = model.ACCTYPE_SHORTNAME.ToString();
            //string accounttypestatus = model.ACCTYPE_STATUS.ToString();
            //string ratingisdefault = model.ACCTYPE_ISDEFAULT.ToString();
            //if (ratingisdefault == "TRUE")
            //{
            //    ratingisdefault = "1";
            //}
            //if (ratingisdefault == "FALSE")
            //{
            //    ratingisdefault = "0";
            //}

            //bool ISPOOLED = model.FBACC__ISPOOLED_BOOL;
            //bool ISCLOSED = model.FBACC_ISCLOSED_BOOL;


            string constr = ConfigurationManager.ConnectionStrings["DAP_NEW"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {

                con.Open();
                // MessageBox.Show ("Connection Open ! ");
                // cnn.Close();
                SqlCommand cmd = new SqlCommand("SP_UPDATE_FUNDBANKACCOUNT", con);    // to be corrected 
                // SqlCommand cmd1 = new SqlCommand("select top 1 * from Customer_IDs where ID_NO ='" + txtCnicNo + "'", oCnn);   
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@FBACC_ID", model.FBACC_ID);
                cmd.Parameters.AddWithValue("@FUND_ID", Int32.Parse(model.FUND_ID));
                cmd.Parameters.AddWithValue("@BANK_ID", Int32.Parse(model.BANK_ID));
                cmd.Parameters.AddWithValue("@BBR_ID", Int32.Parse(model.BBR_ID));
                cmd.Parameters.AddWithValue("@ACCTYPE_ID", Int32.Parse(model.ACCTYPE_ID));
                cmd.Parameters.AddWithValue("@ACCPUR_ID", Int32.Parse(model.ACCPUR_ID));
                cmd.Parameters.AddWithValue("@PBAS_ID", Int32.Parse(model.PBAS_ID));
                cmd.Parameters.AddWithValue("@PFRQ_ID", Int32.Parse(model.PFRQ_ID));
                cmd.Parameters.AddWithValue("@FBACC_ISPOOLED", ISPOOLED);
                cmd.Parameters.AddWithValue("@FBACC_NUMBER", model.FBACC_NUMBER);
                cmd.Parameters.AddWithValue("@FBACC_TITLE", model.FBACC_TITLE);
                cmd.Parameters.AddWithValue("@FBACC_OPENINGDATE", model.Opening_Date);
                cmd.Parameters.AddWithValue("@FBACC_ISCLOSED", ISCLOSED);

                if (model.FBACC_ISCLOSED == "TRUE")
                {
                    cmd.Parameters.AddWithValue("@FBACC_CLOSINGDATE", model.Closing_Date);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@FBACC_CLOSINGDATE", model.Closing_Date);
                }

                //cmd.Parameters.AddWithValue("@FBACC_CLOSINGDATE", model.Closing_Date);
                cmd.Parameters.AddWithValue("@FBACC_TDEBITACCOUNT", model.FBACC_TDEBITACCOUNT);
                cmd.Parameters.AddWithValue("@FBACC_TCREDITACCOUNT", model.FBACC_TCREDITACCOUNT);
                cmd.Parameters.AddWithValue("@FBACC_STATUS", model.FBACC_STATUS);
                cmd.Parameters.AddWithValue("@FBACC_COMPID", model.FBACC_COMPID);
                cmd.Parameters.AddWithValue("@SLAB_ID", model.SLAB_ID);
                cmd.Parameters.AddWithValue("@sUser", user_session);
                s = cmd.ExecuteNonQuery();
                con.Close();
            }

            string message1 = "Successful";
            string message2 = "unsuccessful";
            if (s == 0)
            {
                return Json(message2, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(message1, JsonRequestBehavior.AllowGet);
            }

            //return View();

            //return View(); 
        }
        [HttpPost]
        public ActionResult Export_fundbankaccount_setup() 
        {
            string txtFile = string.Empty;
            DataTable table = new DataTable();
            string constr = ConfigurationManager.ConnectionStrings["DAP_NEW"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                //string query="select RATING_ID,RATING_NAME,RATING_SHORTNAME,RATING_STATUS,RATING_ISDEFAULT from config_rating ";
                using (SqlCommand cmd = new SqlCommand("SP_GET_STP_FUNDBANKACCOUNT", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    con.Open();
                    SqlDataAdapter ds = new SqlDataAdapter(cmd);
                    ds.Fill(table);
                    con.Close();
                }
            }
            List<Fundbankaccount_Setup_pdf> fundbankacct_setup_list = new List<Fundbankaccount_Setup_pdf>();
            for (int i = 0; i < table.Rows.Count; i++)
            {
                fundbankacct_setup_list.Add(new Fundbankaccount_Setup_pdf()
                {
                  // Serial = table.Rows[i]["FBACC_ID"].ToString(),                  
                    FundName = table.Rows[i]["FUND_NAME"].ToString(),                   
                    BankName = table.Rows[i]["BANK_NAME"].ToString(),               
                    BranchName = table.Rows[i]["BBR_NAME"].ToString(),
                    AcctypeName = table.Rows[i]["ACCTYPE_NAME"].ToString(),
                    PbasName = table.Rows[i]["PBAS_NAME"].ToString(),

                    PfrqName = table.Rows[i]["PFRQ_NAME"].ToString(),
                    IsPooled = table.Rows[i]["FBACC_ISPOOLED"].ToString(),
                    AccNumber = table.Rows[i]["FBACC_NUMBER"].ToString(),
                    //Credit = table.Rows[i]["FBACC_TCREDITACCOUNT"].ToString(),
                    //Debit = table.Rows[i]["FBACC_TDEBITACCOUNT"].ToString(),
                    IsClosed = table.Rows[i]["FBACC_ISCLOSED"].ToString(),
                    OpeningDate = table.Rows[i]["FBACC_OPENINGDATE"].ToString(),
                    ClosingDate = table.Rows[i]["FBACC_CLOSINGDATE"].ToString(),
                    Status = table.Rows[i]["FBACC_STATUS"].ToString(),
                    Title = table.Rows[i]["FBACC_TITLE"].ToString(),

                    AccpurName = table.Rows[i]["ACCPUR_NAME"].ToString(),
                   // FBACC_COMPID = table.Rows[i]["FBACC_COMPID"].ToString()
                    // ACCPUR_ID = table.Rows[i]["ACCPUR_ID"].ToString(),
                });
            }
            GridView gv = new GridView();
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "attachment;filename=FundBankAccnt_setup_Report.pdf");
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            StringWriter sw = new StringWriter();
            HtmlTextWriter hw = new HtmlTextWriter(sw);
            gv.DataSource = fundbankacct_setup_list;
            gv.AllowPaging = false;
            gv.DataBind();
            //for (int i = 0; i < gv.Columns.Count; i++)
            //{
            //    //   gv.Columns[0].HeaderText = "Serial";
            //    gv.Columns[i].ItemStyle.Width = 400;
            //    gv.Columns[i].ItemStyle.Wrap = false;
            //}    
            gv.HeaderRow.Style.Add("width", "300%");
            gv.HeaderRow.Style.Add("font-size", "7px");
            gv.Style.Add("text-decoration", "none");
            gv.Style.Add("font-family", "Arial, Helvetica, sans-serif;");
            gv.Style.Add("font-size", "7px");
            //gv.Style.Add("width", "300%");         
            gv.RenderControl(hw);
            StringReader sr = new StringReader(sw.ToString());
            Document pdfDoc = new Document(PageSize.A4.Rotate(), 30f, 30f, 30f, 30f);
            HTMLWorker htmlparser = new HTMLWorker(pdfDoc);
            PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
            pdfDoc.Open();
            htmlparser.Parse(sr);//this is the error line
            pdfDoc.Close();
            Response.Write(pdfDoc);
            Response.End();
            return RedirectToAction("fundbankaccount_setup", "Home");
        }
        public ActionResult profitfrequency_setup()
        {
            if (this.HttpContext.Session["username_session"] != null)
    {



            DataTable table = new DataTable();
            string constr = ConfigurationManager.ConnectionStrings["DAP_NEW"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                //string query="select RATING_ID,RATING_NAME,RATING_SHORTNAME,RATING_STATUS,RATING_ISDEFAULT from config_rating ";
                using (SqlCommand cmd = new SqlCommand("SP_GET_CONFIG_PROFITFREQUENCY", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    con.Open();
                    SqlDataAdapter ds = new SqlDataAdapter(cmd);
                    ds.Fill(table);
                    con.Close();
                }
            }

            List<ProfitFrequency_Setup> pf_list = new List<ProfitFrequency_Setup>();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                //  bankname="Anubhav",shortname="Chaudhary",rating="AA+"

                pf_list.Add(new ProfitFrequency_Setup()
                {
                    PFRQ_ID = table.Rows[i]["PFRQ_ID"].ToString(),
                    PFRQ_NAME = table.Rows[i]["PFRQ_NAME"].ToString(),
                    PFRQ_SHORTNAME = table.Rows[i]["PFRQ_SHORTNAME"].ToString(),
                    PFRQ_STATUS = table.Rows[i]["PFRQ_STATUS"].ToString(),
                    PFRQ_ISDEFAULT = table.Rows[i]["PFRQ_ISDEFAULT"].ToString()

                });
            }

            ProfitFrequency_Setup user1 = new ProfitFrequency_Setup();
            user1.profit_frequency_list = pf_list;
            return View(user1);
    }
            else
            {
                return RedirectToAction("login", "Home");
            }

            }
        [HttpPost]
        public ActionResult profitfrequency_setup(ProfitFrequency_Setup model)
        {
            {

                if (string.IsNullOrEmpty(model.PFRQ_NAME))
                {
                    ModelState.AddModelError("Name", "Kindly Provide Name");
                }
                if (string.IsNullOrEmpty(model.PFRQ_SHORTNAME))
                {
                    ModelState.AddModelError("Name", "Kindly Provide Name");
                }
                if (string.IsNullOrEmpty(model.PFRQ_STATUS))
                {
                    ModelState.AddModelError("Name", "Kindly Provide Name");
                }
                if (ModelState.IsValid)
                {
                    int s = 100;
                    string user_session = this.HttpContext.Session["username_session"].ToString();
                    // string ratingid = model.rating_id.ToString();
                    string accttypename = model.PFRQ_NAME.ToString();
                    string accttypeshortname = model.PFRQ_SHORTNAME.ToString();
                    string accttypestatus = model.PFRQ_STATUS.ToString();
                    //string ratingisdefault = model.rating_isdefault.ToString();
                    bool accttypedefault = model.PFRQ_ISDEFAULT_BOOL;
                    string constr = ConfigurationManager.ConnectionStrings["DAP_NEW"].ConnectionString;
                    using (SqlConnection con = new SqlConnection(constr))
                    {

                        con.Open();
                        // MessageBox.Show ("Connection Open ! ");
                        // cnn.Close();
                        SqlCommand cmd = new SqlCommand("SP_INSERT_CONFIG_PROFITFREQUENCY", con);    // to be corrected 
                        // SqlCommand cmd1 = new SqlCommand("select top 1 * from Customer_IDs where ID_NO ='" + txtCnicNo + "'", oCnn);   
                        cmd.CommandType = CommandType.StoredProcedure;
                        //cmd.Parameters.AddWithValue("@Rating_Id", model.rating_id);
                        cmd.Parameters.AddWithValue("@PFRQ_NAME", model.PFRQ_NAME);
                        cmd.Parameters.AddWithValue("@PFRQ_SHORTNAME", model.PFRQ_SHORTNAME);
                        cmd.Parameters.AddWithValue("@PFRQ_STATUS", model.PFRQ_STATUS);
                        cmd.Parameters.AddWithValue("@PFRQ_ISDEFAULT", model.PFRQ_ISDEFAULT_BOOL);
                        cmd.Parameters.AddWithValue("@sUser", user_session);
                        s = cmd.ExecuteNonQuery();
                        con.Close();
                    }
                    return RedirectToAction("profitfrequency_setup", "Home");
                }
                else
                {
                    DataTable table = new DataTable();
                    string constr = ConfigurationManager.ConnectionStrings["DAP_NEW"].ConnectionString;
                    using (SqlConnection con = new SqlConnection(constr))
                    {
                        //string query="select RATING_ID,RATING_NAME,RATING_SHORTNAME,RATING_STATUS,RATING_ISDEFAULT from config_rating ";
                        using (SqlCommand cmd = new SqlCommand("SP_GET_CONFIG_PROFITFREQUENCY", con))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            con.Open();
                            SqlDataAdapter ds = new SqlDataAdapter(cmd);
                            ds.Fill(table);
                            con.Close();
                        }
                    }

                    List<ProfitFrequency_Setup> pf_list = new List<ProfitFrequency_Setup>();

                    for (int i = 0; i < table.Rows.Count; i++)
                    {
                        //  bankname="Anubhav",shortname="Chaudhary",rating="AA+"

                        pf_list.Add(new ProfitFrequency_Setup()
                        {
                            PFRQ_ID = table.Rows[i]["PFRQ_ID"].ToString(),
                            PFRQ_NAME = table.Rows[i]["PFRQ_NAME"].ToString(),
                            PFRQ_SHORTNAME = table.Rows[i]["PFRQ_SHORTNAME"].ToString(),
                            PFRQ_STATUS = table.Rows[i]["PFRQ_STATUS"].ToString(),
                            PFRQ_ISDEFAULT = table.Rows[i]["PFRQ_ISDEFAULT"].ToString()

                        });
                    }

                    ProfitFrequency_Setup user1 = new ProfitFrequency_Setup();
                    user1.profit_frequency_list = pf_list;
                    return View(user1);
                }

            }







            //return View();
        }
        public JsonResult edit_profitfrequency(ProfitFrequency_Setup model)
        {
            int s = 100;
            string user_session = this.HttpContext.Session["username_session"].ToString();
            string accounttypeid = model.PFRQ_ID.ToString();
            string accounttypename = model.PFRQ_NAME.ToString();
            string accountshortname = model.PFRQ_SHORTNAME.ToString();
            string accounttypestatus = model.PFRQ_STATUS.ToString();
            string ratingisdefault = model.PFRQ_ISDEFAULT.ToString();
            if (ratingisdefault == "TRUE")
            {
                ratingisdefault = "1";
            }
            if (ratingisdefault == "FALSE")
            {
                ratingisdefault = "0";
            }


            string constr = ConfigurationManager.ConnectionStrings["DAP_NEW"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {

                con.Open();
                // MessageBox.Show ("Connection Open ! ");
                // cnn.Close();
                SqlCommand cmd = new SqlCommand("SP_UPDATE_CONFIG_PROFITFREQUENCY", con);    // to be corrected 
                // SqlCommand cmd1 = new SqlCommand("select top 1 * from Customer_IDs where ID_NO ='" + txtCnicNo + "'", oCnn);   
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@PFRQ_ID", model.PFRQ_ID);
                cmd.Parameters.AddWithValue("@PFRQ_NAME", model.PFRQ_NAME);
                cmd.Parameters.AddWithValue("@PFRQ_SHORTNAME", model.PFRQ_SHORTNAME);
                cmd.Parameters.AddWithValue("@PFRQ_STATUS", model.PFRQ_STATUS);
                cmd.Parameters.AddWithValue("@PFRQ_ISDEFAULT", ratingisdefault);
                cmd.Parameters.AddWithValue("@sUser", user_session);
                s = cmd.ExecuteNonQuery();
                con.Close();
            }

            string message1 = "Successful";
            string message2 = "unsuccessful";
            if (s == 0)
            {
                return Json(message2, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(message1, JsonRequestBehavior.AllowGet);
            }



            //return View();

        }
        [HttpPost]
        public ActionResult ExportData_profitfrequency_setup()
        {
            string txtFile = string.Empty;
            DataTable table = new DataTable();
            string constr = ConfigurationManager.ConnectionStrings["DAP_NEW"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                //string query="select RATING_ID,RATING_NAME,RATING_SHORTNAME,RATING_STATUS,RATING_ISDEFAULT from config_rating ";
                using (SqlCommand cmd = new SqlCommand("SP_GET_CONFIG_PROFITFREQUENCY", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    con.Open();
                    SqlDataAdapter ds = new SqlDataAdapter(cmd);
                    ds.Fill(table);
                    con.Close();
                }
            }
            List<ProfitFrequency_Setup_pdf> pf_list = new List<ProfitFrequency_Setup_pdf>();
            for (int i = 0; i < table.Rows.Count; i++)
            {
                pf_list.Add(new ProfitFrequency_Setup_pdf()
                {
                    Serial = table.Rows[i]["PFRQ_ID"].ToString(),
                    Name = table.Rows[i]["PFRQ_NAME"].ToString(),
                    Shortname = table.Rows[i]["PFRQ_SHORTNAME"].ToString(),
                    Status = table.Rows[i]["PFRQ_STATUS"].ToString(),
                    IsDefault = table.Rows[i]["PFRQ_ISDEFAULT"].ToString(),
                    CreatedBy = table.Rows[i]["PFRQ_CREATEDBY"].ToString(),
                    CreatedOn = table.Rows[i]["PFRQ_CREATEDON"].ToString(),
                    ModifiedBy = table.Rows[i]["PFRQ_MODIFIEDBY"].ToString(),
                    ModifiedOn = table.Rows[i]["PFRQ_MODIFIEDON"].ToString(),
                });
            }
            GridView gv = new GridView();
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "attachment;filename=profitfrequency_setup_Report.pdf");
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            StringWriter sw = new StringWriter();
            HtmlTextWriter hw = new HtmlTextWriter(sw);
            gv.DataSource = pf_list;
            gv.AllowPaging = false;
            gv.DataBind();
            //for (int i = 0; i < gv.Columns.Count; i++)
            //{
            //    //   gv.Columns[0].HeaderText = "Serial";
            //    gv.Columns[i].ItemStyle.Width = 400;
            //    gv.Columns[i].ItemStyle.Wrap = false;
            //}    
            gv.HeaderRow.Style.Add("width", "300%");
            gv.HeaderRow.Style.Add("font-size", "7px");
            gv.Style.Add("text-decoration", "none");
            gv.Style.Add("font-family", "Arial, Helvetica, sans-serif;");
            gv.Style.Add("font-size", "7px");
            //gv.Style.Add("width", "300%");         
            gv.RenderControl(hw);
            StringReader sr = new StringReader(sw.ToString());
            Document pdfDoc = new Document(PageSize.A4.Rotate(), 30f, 30f, 30f, 30f);
            HTMLWorker htmlparser = new HTMLWorker(pdfDoc);
            PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
            pdfDoc.Open();
            htmlparser.Parse(sr);//this is the error line
            pdfDoc.Close();
            Response.Write(pdfDoc);
            Response.End();
            return RedirectToAction("profitfrequency_setup", "Home");
        }
        public ActionResult profitbasis_setup()
        {
         
             if (this.HttpContext.Session["username_session"] != null)
    {
            
            DataTable table = new DataTable();
            string constr = ConfigurationManager.ConnectionStrings["DAP_NEW"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                
                using (SqlCommand cmd = new SqlCommand("SP_GET_CONFIG_PROFITBASIS", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    con.Open();
                    SqlDataAdapter ds = new SqlDataAdapter(cmd);
                    ds.Fill(table);
                    con.Close();
                }
            }

            List<ProfitBasis_Setup> pf_list = new List<ProfitBasis_Setup>();
            for (int i = 0; i < table.Rows.Count; i++)
            {
                pf_list.Add(new ProfitBasis_Setup()
                {
                    PBAS_ID = table.Rows[i]["PBAS_ID"].ToString(),
                    PBAS_NAME = table.Rows[i]["PBAS_NAME"].ToString(),
                    PBAS_SHORTNAME = table.Rows[i]["PBAS_SHORTNAME"].ToString(),
                    PBAS_STATUS = table.Rows[i]["PBAS_STATUS"].ToString(),
                    PBAS_ISDEFAULT = table.Rows[i]["PBAS_ISDEFAULT"].ToString()
                });
            }
            ProfitBasis_Setup user1 = new ProfitBasis_Setup();
            user1.profit_basis_list = pf_list;
            return View(user1);
    }
             else
             {
                 return RedirectToAction("login", "Home");
             }		
   
             }
        [HttpPost]
        public ActionResult profitbasis_setup(ProfitBasis_Setup model)
        {
            {

                if (string.IsNullOrEmpty(model.PBAS_NAME))
                {
                    ModelState.AddModelError("Name", "Kindly Provide Name");
                }
                if (string.IsNullOrEmpty(model.PBAS_SHORTNAME))
                {
                    ModelState.AddModelError("Name", "Kindly Provide Name");
                }
                if (string.IsNullOrEmpty(model.PBAS_STATUS))
                {
                    ModelState.AddModelError("Name", "Kindly Provide Name");
                }
                if (ModelState.IsValid)
                {
                    int s = 100;
                    string user_session = this.HttpContext.Session["username_session"].ToString();
                    // string ratingid = model.rating_id.ToString();
                    //string accttypename = model.PFRQ_NAME.ToString();
                    //string accttypeshortname = model.PFRQ_SHORTNAME.ToString();
                    //string accttypestatus = model.PFRQ_STATUS.ToString();
                    //string ratingisdefault = model.rating_isdefault.ToString();
                    bool accttypedefault = model.PBAS_ISDEFAULT_BOOL;
                    string constr = ConfigurationManager.ConnectionStrings["DAP_NEW"].ConnectionString;
                    using (SqlConnection con = new SqlConnection(constr))
                    {

                        con.Open();
                        // MessageBox.Show ("Connection Open ! ");
                        // cnn.Close();
                        SqlCommand cmd = new SqlCommand("SP_INSERT_CONFIG_PROFITBASIS", con);    // to be corrected 
                        // SqlCommand cmd1 = new SqlCommand("select top 1 * from Customer_IDs where ID_NO ='" + txtCnicNo + "'", oCnn);   
                        cmd.CommandType = CommandType.StoredProcedure;
                        //cmd.Parameters.AddWithValue("@Rating_Id", model.rating_id);
                        cmd.Parameters.AddWithValue("@PBAS_NAME", model.PBAS_NAME);
                        cmd.Parameters.AddWithValue("@PBAS_SHORTNAME", model.PBAS_SHORTNAME);
                        cmd.Parameters.AddWithValue("@PBAS_STATUS", model.PBAS_STATUS);
                        cmd.Parameters.AddWithValue("@PBAS_ISDEFAULT", model.PBAS_ISDEFAULT_BOOL);
                        cmd.Parameters.AddWithValue("@sUser", user_session);
                        s = cmd.ExecuteNonQuery();
                        con.Close();
                    }
                    return RedirectToAction("profitbasis_setup", "Home");
                }
                else
                {
                    DataTable table = new DataTable();
                    string constr = ConfigurationManager.ConnectionStrings["DAP_NEW"].ConnectionString;
                    using (SqlConnection con = new SqlConnection(constr))
                    {
                        //string query="select RATING_ID,RATING_NAME,RATING_SHORTNAME,RATING_STATUS,RATING_ISDEFAULT from config_rating ";
                        using (SqlCommand cmd = new SqlCommand("SP_GET_CONFIG_PROFITBASIS", con))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            con.Open();
                            SqlDataAdapter ds = new SqlDataAdapter(cmd);
                            ds.Fill(table);
                            con.Close();
                        }
                    }

                    List<ProfitBasis_Setup> pf_list = new List<ProfitBasis_Setup>();

                    for (int i = 0; i < table.Rows.Count; i++)
                    {
                        //  bankname="Anubhav",shortname="Chaudhary",rating="AA+"

                        pf_list.Add(new ProfitBasis_Setup()
                        {
                            PBAS_ID = table.Rows[i]["PBAS_ID"].ToString(),
                            PBAS_NAME = table.Rows[i]["PBAS_NAME"].ToString(),
                            PBAS_SHORTNAME = table.Rows[i]["PBAS_SHORTNAME"].ToString(),
                            PBAS_STATUS = table.Rows[i]["PBAS_STATUS"].ToString(),
                            PBAS_ISDEFAULT = table.Rows[i]["PBAS_ISDEFAULT"].ToString()

                        });
                    }

                    ProfitBasis_Setup user1 = new ProfitBasis_Setup();
                    user1.profit_basis_list = pf_list;
                    return View(user1);

                }

            }


            //return View();
        }
        public JsonResult edit_profitbasis(ProfitBasis_Setup model)
        {
            int s = 100;
            string user_session = this.HttpContext.Session["username_session"].ToString();
            string accounttypeid = model.PBAS_ID.ToString();
            string accounttypename = model.PBAS_NAME.ToString();
            string accountshortname = model.PBAS_SHORTNAME.ToString();
            string accounttypestatus = model.PBAS_STATUS.ToString();
            string ratingisdefault = model.PBAS_ISDEFAULT.ToString();
            if (ratingisdefault == "TRUE")
            {
                ratingisdefault = "1";
            }
            if (ratingisdefault == "FALSE")
            {
                ratingisdefault = "0";
            }


            string constr = ConfigurationManager.ConnectionStrings["DAP_NEW"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {

                con.Open();
                // MessageBox.Show ("Connection Open ! ");
                // cnn.Close();
                SqlCommand cmd = new SqlCommand("SP_UPDATE_CONFIG_PROFITBASIS", con);    // to be corrected 
                // SqlCommand cmd1 = new SqlCommand("select top 1 * from Customer_IDs where ID_NO ='" + txtCnicNo + "'", oCnn);   
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@PBAS_ID", model.PBAS_ID);
                cmd.Parameters.AddWithValue("@PBAS_NAME", model.PBAS_NAME);
                cmd.Parameters.AddWithValue("@PBAS_SHORTNAME", model.PBAS_SHORTNAME);
                cmd.Parameters.AddWithValue("@PBAS_STATUS", model.PBAS_STATUS);
                cmd.Parameters.AddWithValue("@PBAS_ISDEFAULT", ratingisdefault);
                cmd.Parameters.AddWithValue("@sUser", user_session);
                s = cmd.ExecuteNonQuery();
                con.Close();
            }

            string message1 = "Successful";
            string message2 = "unsuccessful";
            if (s == 0)
            {
                return Json(message2, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(message1, JsonRequestBehavior.AllowGet);
            }



            //return View();

        }
        [HttpPost]
        public ActionResult ExportData_profitbasis_setup()
        {

            string txtFile = string.Empty;
            DataTable table = new DataTable();
            string constr = ConfigurationManager.ConnectionStrings["DAP_NEW"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                //string query="select RATING_ID,RATING_NAME,RATING_SHORTNAME,RATING_STATUS,RATING_ISDEFAULT from config_rating ";
                using (SqlCommand cmd = new SqlCommand("SP_GET_CONFIG_PROFITBASIS",con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    con.Open();
                    SqlDataAdapter ds = new SqlDataAdapter(cmd);
                    ds.Fill(table);
                    con.Close();
                }
            }
            List<ProfitBasis_Setup_pdf> pf_list = new List<ProfitBasis_Setup_pdf>();
            for (int i = 0; i < table.Rows.Count; i++)
            {
                pf_list.Add(new ProfitBasis_Setup_pdf()
                {
                    Serial = table.Rows[i]["PBAS_ID"].ToString(),
                    Name = table.Rows[i]["PBAS_NAME"].ToString(),
                    Shortname = table.Rows[i]["PBAS_SHORTNAME"].ToString(),
                    Status = table.Rows[i]["PBAS_STATUS"].ToString(),
                    IsDefault = table.Rows[i]["PBAS_ISDEFAULT"].ToString(),
                    CreatedBy = table.Rows[i]["PBAS_CREATEDBY"].ToString(),
                    CreatedOn = table.Rows[i]["PBAS_CREATEDON"].ToString(),
                    ModifiedBy = table.Rows[i]["PBAS_MODIFIEDBY"].ToString(),
                    ModifiedOn = table.Rows[i]["PBAS_MODIFIEDON"].ToString(),
                });
            }
            GridView gv = new GridView();
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "attachment;filename=profitbasis_setup_Report.pdf");
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            StringWriter sw = new StringWriter();
            HtmlTextWriter hw = new HtmlTextWriter(sw);
            gv.DataSource = pf_list;
            gv.AllowPaging = false;
            gv.DataBind();
            //for (int i = 0; i < gv.Columns.Count; i++)
            //{
            //    //   gv.Columns[0].HeaderText = "Serial";
            //    gv.Columns[i].ItemStyle.Width = 400;
            //    gv.Columns[i].ItemStyle.Wrap = false;
            //}    
            gv.HeaderRow.Style.Add("width", "300%");
            gv.HeaderRow.Style.Add("font-size", "7px");
            gv.Style.Add("text-decoration", "none");
            gv.Style.Add("font-family", "Arial, Helvetica, sans-serif;");
            gv.Style.Add("font-size", "7px");
            //gv.Style.Add("width", "300%");         
            gv.RenderControl(hw);
            StringReader sr = new StringReader(sw.ToString());
            Document pdfDoc = new Document(PageSize.A4.Rotate(), 30f, 30f, 30f, 30f);
            HTMLWorker htmlparser = new HTMLWorker(pdfDoc);
            PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
            pdfDoc.Open();
            htmlparser.Parse(sr);//this is the error line
            pdfDoc.Close();
            Response.Write(pdfDoc);
            Response.End();
            return RedirectToAction("profitbasis_setup", "Home");
        }
        public ActionResult balamountslabs_setup()
        {
           
            if (this.HttpContext.Session["username_session"] != null)
    {
            
            DataTable table = new DataTable();
            string constr = ConfigurationManager.ConnectionStrings["DAP_NEW"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                //string query="select RATING_ID,RATING_NAME,RATING_SHORTNAME,RATING_STATUS,RATING_ISDEFAULT from config_rating ";
                using (SqlCommand cmd = new SqlCommand("SP_GET_STP_BALAMOUNTSLABS", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    con.Open();
                    SqlDataAdapter ds = new SqlDataAdapter(cmd);
                    ds.Fill(table);
                    con.Close();
                }
            }
            List<BalamountSlabs_Setup> slab_list = new List<BalamountSlabs_Setup>();
            for (int i = 0; i < table.Rows.Count; i++)
            {
                //  bankname="Anubhav",shortname="Chaudhary",rating="AA+"

                slab_list.Add(new BalamountSlabs_Setup()
                {
                    SLAB_ID = table.Rows[i]["SLAB_ID"].ToString(),
                    SLAB_NAME = table.Rows[i]["SLAB_NAME"].ToString(),
                    SLAB_SHORTNAME = table.Rows[i]["SLAB_SHORTNAME"].ToString(),
                    SLAB_STATUS = table.Rows[i]["SLAB_STATUS"].ToString(),
                    SLAB_ISDEFAULT = table.Rows[i]["SLAB_ISDEFAULT"].ToString(),
                    SLAB_AMOUNTFROM = table.Rows[i]["SLAB_AMOUNTFROM"].ToString(),
                    SLAB_AMOUNTTO = table.Rows[i]["SLAB_AMOUNTTO"].ToString(),
                    BANK_ID = table.Rows[i]["BANK_ID"].ToString(),
                    BANK_NAME = table.Rows[i]["BANK_NAME"].ToString()

                });
            }

                  DataTable table2 = new DataTable();
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = "select * from STP_BANK";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    SqlDataAdapter ds = new SqlDataAdapter(cmd);
                    ds.Fill(table2);
                }
            }
            List<BalamountSlabs_Setup> bank_list = new List<BalamountSlabs_Setup>();

            for (int i = 0; i < table2.Rows.Count; i++)
            {
                //  bankname="Anubhav",shortname="Chaudhary",rating="AA+"

                bank_list.Add(new BalamountSlabs_Setup()
                {
                    BANK_NAME = table2.Rows[i]["BANK_NAME"].ToString(),
                    BANK_ID = table2.Rows[i]["BANK_ID"].ToString()
                });
            }



            BalamountSlabs_Setup user1 = new BalamountSlabs_Setup();
            user1.balamounts_slab_list = slab_list;
            user1.bank_list = bank_list;
            return View(user1);
    }
            else
            {
                return RedirectToAction("login", "Home");
            }

        }
        [HttpPost]
        public ActionResult balamountslabs_setup(BalamountSlabs_Setup model)
        {
            {

                if (string.IsNullOrEmpty(model.SLAB_NAME))
                {
                    ModelState.AddModelError("Name", "Kindly Provide Name");
                }
                if (string.IsNullOrEmpty(model.SLAB_SHORTNAME))
                {
                    ModelState.AddModelError("Name", "Kindly Provide Name");
                }

                if (string.IsNullOrEmpty(model.SLAB_AMOUNTFROM))
                {
                    ModelState.AddModelError("Name", "Kindly Provide Name");
                }

                if (string.IsNullOrEmpty(model.SLAB_AMOUNTTO))
                {
                    ModelState.AddModelError("Name", "Kindly Provide Name");
                }
                if (string.IsNullOrEmpty(model.SLAB_STATUS))
                {
                    ModelState.AddModelError("Name", "Kindly Provide Name");
                }
                
                if (ModelState.IsValid)
                {
                    int s = 100;
                    string user_session = this.HttpContext.Session["username_session"].ToString();
                    // string ratingid = model.rating_id.ToString();
                    //string accttypename = model.PFRQ_NAME.ToString();
                    //string accttypeshortname = model.PFRQ_SHORTNAME.ToString();
                    //string accttypestatus = model.PFRQ_STATUS.ToString();
                    //string ratingisdefault = model.rating_isdefault.ToString();
                    bool accttypedefault = model.SLAB_ISDEFAULT_BOOL;
                    string constr = ConfigurationManager.ConnectionStrings["DAP_NEW"].ConnectionString;
                    using (SqlConnection con = new SqlConnection(constr))
                    {

                        con.Open();
                        // MessageBox.Show ("Connection Open ! ");
                        // cnn.Close();
                        SqlCommand cmd = new SqlCommand("SP_INSERT_STP_BALAMOUNTSLABS", con);    // to be corrected 
                        // SqlCommand cmd1 = new SqlCommand("select top 1 * from Customer_IDs where ID_NO ='" + txtCnicNo + "'", oCnn);   
                        cmd.CommandType = CommandType.StoredProcedure;
                        //cmd.Parameters.AddWithValue("@Rating_Id", model.rating_id);
                        cmd.Parameters.AddWithValue("@SLAB_NAME", model.SLAB_NAME);
                        cmd.Parameters.AddWithValue("@SLAB_SHORTNAME", model.SLAB_SHORTNAME);
                        cmd.Parameters.AddWithValue("@SLAB_STATUS", model.SLAB_STATUS);
                        cmd.Parameters.AddWithValue("@SLAB_ISDEFAULT", model.SLAB_ISDEFAULT_BOOL);
                        cmd.Parameters.AddWithValue("@SLAB_AMOUNTFROM", model.SLAB_AMOUNTFROM);
                        cmd.Parameters.AddWithValue("@SLAB_AMOUNTTO", model.SLAB_AMOUNTTO);
                        cmd.Parameters.AddWithValue("@sUser", user_session);
                        cmd.Parameters.AddWithValue("@BANK_ID", model.BANK_ID);

                        
                        s = cmd.ExecuteNonQuery();
                        con.Close();
                    }
                    return RedirectToAction("balamountslabs_setup", "Home");
                }
                else
                {
                    DataTable table = new DataTable();
                    string constr = ConfigurationManager.ConnectionStrings["DAP_NEW"].ConnectionString;
                    using (SqlConnection con = new SqlConnection(constr))
                    {
                        //string query="select RATING_ID,RATING_NAME,RATING_SHORTNAME,RATING_STATUS,RATING_ISDEFAULT from config_rating ";
                        using (SqlCommand cmd = new SqlCommand("SP_GET_STP_BALAMOUNTSLABS", con))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            con.Open();
                            SqlDataAdapter ds = new SqlDataAdapter(cmd);
                            ds.Fill(table);
                            con.Close();
                        }
                    }
                    List<BalamountSlabs_Setup> slab_list = new List<BalamountSlabs_Setup>();
                    for (int i = 0; i < table.Rows.Count; i++)
                    {
                        //  bankname="Anubhav",shortname="Chaudhary",rating="AA+"

                        slab_list.Add(new BalamountSlabs_Setup()
                        {
                            SLAB_ID = table.Rows[i]["SLAB_ID"].ToString(),
                            SLAB_NAME = table.Rows[i]["SLAB_NAME"].ToString(),
                            SLAB_SHORTNAME = table.Rows[i]["SLAB_SHORTNAME"].ToString(),
                            SLAB_STATUS = table.Rows[i]["SLAB_STATUS"].ToString(),
                            SLAB_ISDEFAULT = table.Rows[i]["SLAB_ISDEFAULT"].ToString(),
                            SLAB_AMOUNTFROM = table.Rows[i]["SLAB_AMOUNTFROM"].ToString(),
                            SLAB_AMOUNTTO = table.Rows[i]["SLAB_AMOUNTTO"].ToString(),
                            BANK_ID = table.Rows[i]["BANK_ID"].ToString(),
                            BANK_NAME = table.Rows[i]["BANK_NAME"].ToString()
                        });
                    }
                    DataTable table1 = new DataTable();
                    using (SqlConnection con = new SqlConnection(constr))
                    {
                       
                        //string query="select RATING_ID,RATING_NAME,RATING_SHORTNAME,RATING_STATUS,RATING_ISDEFAULT from config_rating ";
                        using (SqlCommand cmd = new SqlCommand("SELECT * FROM STP_BANK", con))
                        {
                            cmd.CommandType = CommandType.Text;
                            con.Open();
                            SqlDataAdapter ds = new SqlDataAdapter(cmd);
                            ds.Fill(table1);
                            con.Close();
                        }
                    }
                    List<BalamountSlabs_Setup> BANK_LIST = new List<BalamountSlabs_Setup>();
                    for (int i = 0; i < table1.Rows.Count; i++)
                    {
                        //  bankname="Anubhav",shortname="Chaudhary",rating="AA+"

                        BANK_LIST.Add(new BalamountSlabs_Setup()
                        {                            
                            BANK_ID = table1.Rows[i]["BANK_ID"].ToString(),
                            BANK_NAME = table1.Rows[i]["BANK_NAME"].ToString()
                        });
                    }


                    BalamountSlabs_Setup user1 = new BalamountSlabs_Setup();
                    user1.balamounts_slab_list = slab_list;
                    user1.bank_list = BANK_LIST;


                    return View(user1);

                }

            }


            //return View();
        }
        public JsonResult edit_balamountslabs(BalamountSlabs_Setup model)
        {
            int s = 100;
            string user_session = this.HttpContext.Session["username_session"].ToString();
            //string accounttypeid = model.PBAS_ID.ToString();
            //string accounttypename = model.PBAS_NAME.ToString();
            //string accountshortname = model.PBAS_SHORTNAME.ToString();
            //string accounttypestatus = model.PBAS_STATUS.ToString();
            string ratingisdefault = model.SLAB_ISDEFAULT.ToString();
            if (ratingisdefault == "TRUE")
            {
                ratingisdefault = "1";
            }
            if (ratingisdefault == "FALSE")
            {
                ratingisdefault = "0";
            }


            string constr = ConfigurationManager.ConnectionStrings["DAP_NEW"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {

                con.Open();
                // MessageBox.Show ("Connection Open ! ");
                // cnn.Close();
                SqlCommand cmd = new SqlCommand("SP_UPDATE_STP_BALAMOUNTSLABS", con);    // to be corrected 
                // SqlCommand cmd1 = new SqlCommand("select top 1 * from Customer_IDs where ID_NO ='" + txtCnicNo + "'", oCnn);   
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@SLAB_ID", model.SLAB_ID);
                cmd.Parameters.AddWithValue("@SLAB_NAME", model.SLAB_NAME);
                cmd.Parameters.AddWithValue("@SLAB_SHORTNAME", model.SLAB_SHORTNAME);
                cmd.Parameters.AddWithValue("@SLAB_STATUS", model.SLAB_STATUS);
                cmd.Parameters.AddWithValue("@SLAB_ISDEFAULT", ratingisdefault);
                cmd.Parameters.AddWithValue("@SLAB_AMOUNTFROM", model.SLAB_AMOUNTFROM);
                cmd.Parameters.AddWithValue("@SLAB_AMOUNTTO", model.SLAB_AMOUNTTO);
                cmd.Parameters.AddWithValue("@BANK_ID", model.BANK_ID);

                cmd.Parameters.AddWithValue("@sUser", user_session);
                s = cmd.ExecuteNonQuery();
                con.Close();
            }

            string message1 = "Successful";
            string message2 = "unsuccessful";
            if (s == 0)
            {
                return Json(message2, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(message1, JsonRequestBehavior.AllowGet);
            }



            //return View();

        }
        public ActionResult ExportData_balamountslabs_setup()
        {

            string txtFile = string.Empty;
            DataTable table = new DataTable();
            string constr = ConfigurationManager.ConnectionStrings["DAP_NEW"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                //string query="select RATING_ID,RATING_NAME,RATING_SHORTNAME,RATING_STATUS,RATING_ISDEFAULT from config_rating ";
                using (SqlCommand cmd = new SqlCommand("SP_GET_STP_BALAMOUNTSLABS", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    con.Open();
                    SqlDataAdapter ds = new SqlDataAdapter(cmd);
                    ds.Fill(table);
                    con.Close();
                }
            }
            List<BalamountSlabs_Setup_pdf> slab_list = new List<BalamountSlabs_Setup_pdf>();
            for (int i = 0; i < table.Rows.Count; i++)
            {
                slab_list.Add(new BalamountSlabs_Setup_pdf()
                {
                    Serial = table.Rows[i]["SLAB_ID"].ToString(),
                    Name = table.Rows[i]["SLAB_NAME"].ToString(),
                    Shortname = table.Rows[i]["SLAB_SHORTNAME"].ToString(),
                    Status = table.Rows[i]["SLAB_STATUS"].ToString(),
                    IsDefault = table.Rows[i]["SLAB_ISDEFAULT"].ToString(),
                    AmountFrom = table.Rows[i]["SLAB_AMOUNTFROM"].ToString(),
                    AmountTO = table.Rows[i]["SLAB_AMOUNTTO"].ToString(),
                    CreatedBy = table.Rows[i]["SLAB_CREATEDBY"].ToString(),    /// WORK REQUIRED ON MODEL CLASS 
                    CreatedOn = table.Rows[i]["SLAB_CREATEDON"].ToString(),
                    ModifiedBy = table.Rows[i]["SLAB_MODIFIEDBY"].ToString(),
                    ModifiedOn = table.Rows[i]["SLAB_MODIFIEDON"].ToString(),
                });
            }
            GridView gv = new GridView();
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "attachment;filename=balamountslabs_setup_Report.pdf");
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            StringWriter sw = new StringWriter();
            HtmlTextWriter hw = new HtmlTextWriter(sw);
            gv.DataSource = slab_list;
            gv.AllowPaging = false;
            gv.DataBind();
            //for (int i = 0; i < gv.Columns.Count; i++)
            //{
            //    //   gv.Columns[0].HeaderText = "Serial";
            //    gv.Columns[i].ItemStyle.Width = 400;
            //    gv.Columns[i].ItemStyle.Wrap = false;
            //}    
            gv.HeaderRow.Style.Add("width", "300%");
            gv.HeaderRow.Style.Add("font-size", "7px");
            gv.Style.Add("text-decoration", "none");
            gv.Style.Add("font-family", "Arial, Helvetica, sans-serif;");
            gv.Style.Add("font-size", "7px");
            //gv.Style.Add("width", "300%");         
            gv.RenderControl(hw);
            StringReader sr = new StringReader(sw.ToString());
            Document pdfDoc = new Document(PageSize.A4.Rotate(), 30f, 30f, 30f, 30f);
            HTMLWorker htmlparser = new HTMLWorker(pdfDoc);
            PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
            pdfDoc.Open();
            htmlparser.Parse(sr);//this is the error line
            pdfDoc.Close();
            Response.Write(pdfDoc);
            Response.End();
            return RedirectToAction("balamountslabs_setup", "Home");

        }               
        public ActionResult profitrate_setup()
        {
         if (this.HttpContext.Session["username_session"] != null)
          {
            string constr = ConfigurationManager.ConnectionStrings["DAP_NEW"].ConnectionString;
            DataTable table = new DataTable();
            using (SqlConnection con = new SqlConnection(constr))
            {
                //string query = "SELECT * FROM CONFIG_ACCOUNTPURPOSE ";
                using (SqlCommand cmd = new SqlCommand("SP_GET_STP_PROFITRATE", con))
                {
                    SqlDataAdapter ds = new SqlDataAdapter(cmd);
                    ds.Fill(table);
                }
            }
            List<ProfitRate_Setup> pr_setup_list = new List<ProfitRate_Setup>();
            for (int i = 0; i < table.Rows.Count; i++)
            {
                pr_setup_list.Add(new ProfitRate_Setup()
                {
                    PTRATE_ID = table.Rows[i]["PTRATE_ID"].ToString(),
                    SLAB_ID = table.Rows[i]["SLAB_ID"].ToString(),
                    SLAB_NAME = table.Rows[i]["SLAB_NAME"].ToString(),
                    //FUND_ID = table.Rows[i]["FUND_ID"].ToString(),
                    //FUND_NAME = table.Rows[i]["FUND_NAME"].ToString(),
                    BANK_ID = table.Rows[i]["BANK_ID"].ToString(),
                    BANK_NAME = table.Rows[i]["BANK_NAME"].ToString(),
                    BBR_ID = table.Rows[i]["BBR_ID"].ToString(),
                    BBR_NAME = table.Rows[i]["BBR_NAME"].ToString(),
                    ACCTYPE_ID = table.Rows[i]["ACCTYPE_ID"].ToString(),
                    ACCTYPE_NAME = table.Rows[i]["ACCTYPE_NAME"].ToString(),
                    PTRATE_PERCENT = table.Rows[i]["PTRATE_PERCENT"].ToString(),
                    PTRATE_EFFECTIVEFROM = table.Rows[i]["PTRATE_EFFECTIVEFROM"].ToString(),
                    PTRATE_EFFECTIVETO = table.Rows[i]["PTRATE_EFFECTIVETO"].ToString(),
                    PTRATE_STATUS = table.Rows[i]["PTRATE_STATUS"].ToString(),
                    ACCPUR_ID = table.Rows[i]["ACCPUR_ID"].ToString(),
                    ACCPUR_NAME = table.Rows[i]["ACCPUR_NAME"].ToString(),
                    PTRATE_COMPID = table.Rows[i]["PTRATE_COMPID"].ToString(),
                    COMP_NAME = table.Rows[i]["FUND_NAME"].ToString()
                    // ACCPUR_ID = table.Rows[i]["ACCPUR_ID"].ToString(),
                });
            }

            //////////////////////////////////############################////////////////////////

            //string constr = ConfigurationManager.ConnectionStrings["DAP_NEW"].ConnectionString;
            DataTable table1 = new DataTable();
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = "select * from STP_FUND";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    SqlDataAdapter ds = new SqlDataAdapter(cmd);
                    ds.Fill(table1);
                }
            }
            List<ProfitRate_Setup> fund_list = new List<ProfitRate_Setup>();

            for (int i = 0; i < table1.Rows.Count; i++)
            {
                //  bankname="Anubhav",shortname="Chaudhary",rating="AA+"

                fund_list.Add(new ProfitRate_Setup()
                {
                    //FUND_ID = table1.Rows[i]["FUND_ID"].ToString(),
                    COMP_NAME = table1.Rows[i]["FUND_NAME"].ToString(),
                    PTRATE_COMPID = table1.Rows[i]["FUND_COMPID"].ToString()
                });
            }

            //////////////////////////////////############################////////////////////////

            DataTable table2 = new DataTable();
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = "select * from STP_BANK";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    SqlDataAdapter ds = new SqlDataAdapter(cmd);
                    ds.Fill(table2);
                }
            }
            List<ProfitRate_Setup> bank_list = new List<ProfitRate_Setup>();

            for (int i = 0; i < table2.Rows.Count; i++)
            {
                //  bankname="Anubhav",shortname="Chaudhary",rating="AA+"

                bank_list.Add(new ProfitRate_Setup()
                {
                    BANK_NAME = table2.Rows[i]["BANK_NAME"].ToString(),
                    BANK_ID = table2.Rows[i]["BANK_ID"].ToString()
                });
            }

            //////////////////////////////////############################////////////////////////

            DataTable table3 = new DataTable();
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = "SELECT * FROM STP_BANKBRANCH";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    SqlDataAdapter ds = new SqlDataAdapter(cmd);
                    ds.Fill(table3);
                }
            }
            List<ProfitRate_Setup> branch_list = new List<ProfitRate_Setup>();

            for (int i = 0; i < table3.Rows.Count; i++)
            {
                //  bankname="Anubhav",shortname="Chaudhary",rating="AA+"

                branch_list.Add(new ProfitRate_Setup()
                {
                    BBR_ID = table3.Rows[i]["BBR_ID"].ToString(),
                    BBR_NAME = table3.Rows[i]["BBR_NAME"].ToString()
                });
            }

            //////////////////////////////////############################////////////////////////

            DataTable table4 = new DataTable();
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = "SELECT * FROM CONFIG_ACCOUNTTYPE";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    SqlDataAdapter ds = new SqlDataAdapter(cmd);
                    ds.Fill(table4);
                }
            }
            List<ProfitRate_Setup> acctype_list = new List<ProfitRate_Setup>();

            for (int i = 0; i < table4.Rows.Count; i++)
            {                
                acctype_list.Add(new ProfitRate_Setup()
                {
                    ACCTYPE_ID = table4.Rows[i]["ACCTYPE_ID"].ToString(),
                    ACCTYPE_NAME = table4.Rows[i]["ACCTYPE_NAME"].ToString()
                });
            }

            //////////////////////////////////############################////////////////////////

            DataTable table5 = new DataTable();
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = "select * from CONFIG_ACCOUNTPURPOSE";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    SqlDataAdapter ds = new SqlDataAdapter(cmd);
                    ds.Fill(table5);
                }
            }
            List<ProfitRate_Setup> accpur_list = new List<ProfitRate_Setup>();

            for (int i = 0; i < table5.Rows.Count; i++)
            {
                accpur_list.Add(new ProfitRate_Setup()
                {
                    ACCPUR_ID = table5.Rows[i]["ACCPUR_ID"].ToString(),
                    ACCPUR_NAME = table5.Rows[i]["ACCPUR_NAME"].ToString()
                });
            }

            //////////////////////////////////############################////////////////////////

            DataTable table6 = new DataTable();
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = "select * from STP_BALAMOUNTSLABS";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    SqlDataAdapter ds = new SqlDataAdapter(cmd);
                    ds.Fill(table6);
                }
            }
            List<ProfitRate_Setup> SLAB_list = new List<ProfitRate_Setup>();

            for (int i = 0; i < table6.Rows.Count; i++)
            {
                //  bankname="Anubhav",shortname="Chaudhary",rating="AA+"

                SLAB_list.Add(new ProfitRate_Setup()
                {
                    SLAB_ID = table6.Rows[i]["SLAB_ID"].ToString(),
                    SLAB_NAME = table6.Rows[i]["SLAB_NAME"].ToString()
                });
            }

            //////////////////////////////////############################////////////////////////
            ProfitRate_Setup user1 = new ProfitRate_Setup();
            user1.fund_list = fund_list;
            user1.branch_list = branch_list;
            user1.bank_list = bank_list;
            user1.acctype_list = acctype_list;
            user1.accpur_list = accpur_list;
            user1.slab_list = SLAB_list;
            user1.profit_rate_list = pr_setup_list;
            user1.EFFECTIVEFROM = DateTime.Parse("01/01/1900");
            user1.EFFECTIVETO = DateTime.Parse("01/01/1900");
             return View(user1);
          }
         else
         {
             return RedirectToAction("login", "Home");
         }

         }
        [HttpPost]
        public ActionResult profitrate_setup(ProfitRate_Setup model)
        {
            int s = 100;
            string user_session = this.HttpContext.Session["username_session"].ToString();
            string constr = ConfigurationManager.ConnectionStrings["DAP_NEW"].ConnectionString;
            if (string.IsNullOrEmpty(model.PTRATE_PERCENT))
            {
                ModelState.AddModelError("Name", "Kindly Provide Name");
            }
            DateTime dt1 = model.EFFECTIVEFROM;
            DateTime dt2 = model.EFFECTIVETO;
            DateTime defaultdate = DateTime.Parse("01/01/0001");   
            if (model.EFFECTIVETO == DateTime.Parse("01/01/0001"))
            {
                model.EFFECTIVETO = DateTime.Parse("01/01/1900");

                // ModelState.AddModelError("Name", "kindly enter closing date");
            }
            if (model.EFFECTIVEFROM == DateTime.Parse("01/01/0001"))
            {
                model.EFFECTIVEFROM = DateTime.Parse("01/01/1900");

                //ModelState.AddModelError("Name", "kindly enter closing date");
            }

            if (ModelState.IsValid)
            {



                using (SqlConnection con = new SqlConnection(constr))
                {
                    con.Open();
                    // MessageBox.Show ("Connection Open ! ");
                    // cnn.Close();
                    SqlCommand cmd = new SqlCommand("SP_INSERT_PROFITRATE", con);    // to be corrected 
                    // SqlCommand cmd1 = new SqlCommand("select top 1 * from Customer_IDs where ID_NO ='" + txtCnicNo + "'", oCnn);   
                    cmd.CommandType = CommandType.StoredProcedure;
                    //cmd.Parameters.AddWithValue("@Rating_Id", model.rating_id);
                    //cmd.Parameters.AddWithValue("@PTRATE_ID", Int32.Parse(model.PTRATE_ID));
                    cmd.Parameters.AddWithValue("@BANK_ID", Int32.Parse(model.BANK_ID));
                    cmd.Parameters.AddWithValue("@BBR_ID", Int32.Parse(model.BBR_ID));
                    cmd.Parameters.AddWithValue("@ACCTYPE_ID", Int32.Parse(model.ACCTYPE_ID));
                    cmd.Parameters.AddWithValue("@ACCPUR_ID", Int32.Parse(model.ACCPUR_ID));
                    cmd.Parameters.AddWithValue("@SLAB_ID", Int32.Parse(model.SLAB_ID));
                    cmd.Parameters.AddWithValue("@PTRATE_PERCENT",model.PTRATE_PERCENT);
                    cmd.Parameters.AddWithValue("@PTRATE_EFFECTIVEFROM", model.EFFECTIVEFROM);
                    cmd.Parameters.AddWithValue("@PTRATE_EFFECTIVETO", model.EFFECTIVETO);
                    cmd.Parameters.AddWithValue("@PTRATE_STATUS", model.PTRATE_STATUS);
                    cmd.Parameters.AddWithValue("@PTRATE_COMPID", model.PTRATE_COMPID);
                    //cmd.Parameters.AddWithValue("@FBACC_OPENINGDATE", model.Opening_Date);
                    //cmd.Parameters.AddWithValue("@FBACC_ISCLOSED", ISCLOSED);
                    //cmd.Parameters.AddWithValue("@FBACC_CLOSINGDATE", model.Closing_Date);     
                    cmd.Parameters.AddWithValue("@sUser", user_session);
                    s = cmd.ExecuteNonQuery();
                    con.Close();
                }




                return RedirectToAction("profitrate_setup", "Home");
            }

            else
            {

                // string constr = ConfigurationManager.ConnectionStrings["DAP_NEW"].ConnectionString;
                DataTable table = new DataTable();
                using (SqlConnection con = new SqlConnection(constr))
                {
                    //string query = "SELECT * FROM CONFIG_ACCOUNTPURPOSE ";
                    using (SqlCommand cmd = new SqlCommand("SP_GET_STP_PROFITRATE", con))
                    {
                        SqlDataAdapter ds = new SqlDataAdapter(cmd);
                        ds.Fill(table);
                    }
                }
                List<ProfitRate_Setup> pr_setup_list = new List<ProfitRate_Setup>();
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    pr_setup_list.Add(new ProfitRate_Setup()
                    {
                        PTRATE_ID = table.Rows[i]["PTRATE_ID"].ToString(),
                        SLAB_ID = table.Rows[i]["SLAB_ID"].ToString(),
                        SLAB_NAME = table.Rows[i]["SLAB_NAME"].ToString(),
                        //FUND_ID = table.Rows[i]["FUND_ID"].ToString(),
                        //FUND_NAME = table.Rows[i]["FUND_NAME"].ToString(),
                        BANK_ID = table.Rows[i]["BANK_ID"].ToString(),
                        BANK_NAME = table.Rows[i]["BANK_NAME"].ToString(),
                        BBR_ID = table.Rows[i]["BBR_ID"].ToString(),
                        BBR_NAME = table.Rows[i]["BBR_NAME"].ToString(),
                        ACCTYPE_ID = table.Rows[i]["ACCTYPE_ID"].ToString(),
                        ACCTYPE_NAME = table.Rows[i]["ACCTYPE_NAME"].ToString(),
                        PTRATE_PERCENT = table.Rows[i]["PTRATE_PERCENT"].ToString(),
                        PTRATE_EFFECTIVEFROM = table.Rows[i]["PTRATE_EFFECTIVEFROM"].ToString(),
                        PTRATE_EFFECTIVETO = table.Rows[i]["PTRATE_EFFECTIVETO"].ToString(),
                        PTRATE_STATUS = table.Rows[i]["PTRATE_STATUS"].ToString(),
                        ACCPUR_ID = table.Rows[i]["ACCPUR_ID"].ToString(),
                        ACCPUR_NAME = table.Rows[i]["ACCPUR_NAME"].ToString(),
                        PTRATE_COMPID = table.Rows[i]["PTRATE_COMPID"].ToString(),
                        COMP_NAME = table.Rows[i]["FUND_NAME"].ToString()
                        // ACCPUR_ID = table.Rows[i]["ACCPUR_ID"].ToString(),
                    });
                }

                //////////////////////////////////############################////////////////////////

                //string constr = ConfigurationManager.ConnectionStrings["DAP_NEW"].ConnectionString;
                DataTable table1 = new DataTable();
                using (SqlConnection con = new SqlConnection(constr))
                {
                    string query = "select * from STP_FUND";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        SqlDataAdapter ds = new SqlDataAdapter(cmd);
                        ds.Fill(table1);
                    }
                }
                List<ProfitRate_Setup> fund_list = new List<ProfitRate_Setup>();

                for (int i = 0; i < table1.Rows.Count; i++)
                {
                    //  bankname="Anubhav",shortname="Chaudhary",rating="AA+"

                    fund_list.Add(new ProfitRate_Setup()
                    {
                        //FUND_ID = table1.Rows[i]["FUND_ID"].ToString(),
                        COMP_NAME = table1.Rows[i]["FUND_NAME"].ToString(),
                        PTRATE_COMPID = table1.Rows[i]["FUND_COMPID"].ToString()
                    });
                }

                //////////////////////////////////############################////////////////////////

                DataTable table2 = new DataTable();
                using (SqlConnection con = new SqlConnection(constr))
                {
                    string query = "select * from STP_BANK";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        SqlDataAdapter ds = new SqlDataAdapter(cmd);
                        ds.Fill(table2);
                    }
                }
                List<ProfitRate_Setup> bank_list = new List<ProfitRate_Setup>();

                for (int i = 0; i < table2.Rows.Count; i++)
                {
                    //  bankname="Anubhav",shortname="Chaudhary",rating="AA+"

                    bank_list.Add(new ProfitRate_Setup()
                    {
                        BANK_NAME = table2.Rows[i]["BANK_NAME"].ToString(),
                        BANK_ID = table2.Rows[i]["BANK_ID"].ToString()
                    });
                }

                //////////////////////////////////############################////////////////////////

                DataTable table3 = new DataTable();
                using (SqlConnection con = new SqlConnection(constr))
                {
                    string query = "SELECT * FROM STP_BANKBRANCH";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        SqlDataAdapter ds = new SqlDataAdapter(cmd);
                        ds.Fill(table3);
                    }
                }
                List<ProfitRate_Setup> branch_list = new List<ProfitRate_Setup>();

                for (int i = 0; i < table3.Rows.Count; i++)
                {
                    //  bankname="Anubhav",shortname="Chaudhary",rating="AA+"

                    branch_list.Add(new ProfitRate_Setup()
                    {
                        BBR_ID = table3.Rows[i]["BBR_ID"].ToString(),
                        BBR_NAME = table3.Rows[i]["BBR_NAME"].ToString()
                    });
                }

                //////////////////////////////////############################////////////////////////

                DataTable table4 = new DataTable();
                using (SqlConnection con = new SqlConnection(constr))
                {
                    string query = "SELECT * FROM CONFIG_ACCOUNTTYPE";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        SqlDataAdapter ds = new SqlDataAdapter(cmd);
                        ds.Fill(table4);
                    }
                }
                List<ProfitRate_Setup> acctype_list = new List<ProfitRate_Setup>();

                for (int i = 0; i < table4.Rows.Count; i++)
                {
                    //  bankname="Anubhav",shortname="Chaudhary",rating="AA+"

                    acctype_list.Add(new ProfitRate_Setup()
                    {
                        ACCTYPE_ID = table4.Rows[i]["ACCTYPE_ID"].ToString(),
                        ACCTYPE_NAME = table4.Rows[i]["ACCTYPE_NAME"].ToString()
                    });
                }

                //////////////////////////////////############################////////////////////////

                DataTable table5 = new DataTable();
                using (SqlConnection con = new SqlConnection(constr))
                {
                    string query = "select * from CONFIG_ACCOUNTPURPOSE";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        SqlDataAdapter ds = new SqlDataAdapter(cmd);
                        ds.Fill(table5);
                    }
                }
                List<ProfitRate_Setup> accpur_list = new List<ProfitRate_Setup>();

                for (int i = 0; i < table5.Rows.Count; i++)
                {
                    //  bankname="Anubhav",shortname="Chaudhary",rating="AA+"

                    accpur_list.Add(new ProfitRate_Setup()
                    {
                        ACCPUR_ID = table5.Rows[i]["ACCPUR_ID"].ToString(),
                        ACCPUR_NAME = table5.Rows[i]["ACCPUR_NAME"].ToString()
                    });
                }

                //////////////////////////////////############################////////////////////////

                DataTable table6 = new DataTable();
                using (SqlConnection con = new SqlConnection(constr))
                {
                    string query = "select * from STP_BALAMOUNTSLABS";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        SqlDataAdapter ds = new SqlDataAdapter(cmd);
                        ds.Fill(table6);
                    }
                }
                List<ProfitRate_Setup> SLAB_list = new List<ProfitRate_Setup>();

                for (int i = 0; i < table6.Rows.Count; i++)
                {
                    //  bankname="Anubhav",shortname="Chaudhary",rating="AA+"

                    SLAB_list.Add(new ProfitRate_Setup()
                    {
                        SLAB_ID = table6.Rows[i]["SLAB_ID"].ToString(),
                        SLAB_NAME = table6.Rows[i]["SLAB_NAME"].ToString()
                    });
                }

                //////////////////////////////////############################////////////////////////

                //DataTable table7 = new DataTable();
                //using (SqlConnection con = new SqlConnection(constr))
                //{
                //    string query = "select * from CONFIG_PROFITFREQUENCY";
                //    using (SqlCommand cmd = new SqlCommand(query, con))
                //    {
                //        SqlDataAdapter ds = new SqlDataAdapter(cmd);
                //        ds.Fill(table7);
                //    }
                //}
                //List<Fundbankaccount_Setup> pfrq_list = new List<Fundbankaccount_Setup>();

                //for (int i = 0; i < table7.Rows.Count; i++)
                //{
                //    //  bankname="Anubhav",shortname="Chaudhary",rating="AA+"

                //    pfrq_list.Add(new Fundbankaccount_Setup()
                //    {
                //        PFRQ_ID = table7.Rows[i]["PFRQ_ID"].ToString(),
                //        PFRQ_NAME = table7.Rows[i]["PFRQ_NAME"].ToString()
                //    });
                //}

                //////////////////////////////////############################////////////////////////


                ProfitRate_Setup user1 = new ProfitRate_Setup();
                user1.fund_list = fund_list;
                user1.branch_list = branch_list;
                user1.bank_list = bank_list;
                user1.acctype_list = acctype_list;
                user1.accpur_list = accpur_list;
                user1.slab_list = SLAB_list;
                user1.profit_rate_list = pr_setup_list;
                return View(user1);

            }
            //return View();
        }
        public JsonResult edit_profitrate(ProfitRate_Setup model)
        {

            DateTime efr = DateTime.Parse(model.EFFECTIVEFROM.ToShortDateString());
            DateTime eto = DateTime.Parse(model.EFFECTIVETO.ToShortDateString()); 

            if(eto>=efr)
            {
            int s = 100;
            string user_session = this.HttpContext.Session["username_session"].ToString();
           
            string constr = ConfigurationManager.ConnectionStrings["DAP_NEW"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {

                con.Open();
                // MessageBox.Show ("Connection Open ! ");
                // cnn.Close();
                SqlCommand cmd = new SqlCommand("SP_UPDATE_STP_PROFITRATE", con);    // to be corrected 
                // SqlCommand cmd1 = new SqlCommand("select top 1 * from Customer_IDs where ID_NO ='" + txtCnicNo + "'", oCnn);   
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@PTRATE_ID", model.PTRATE_ID);
                cmd.Parameters.AddWithValue("@PTRATE_EFFECTIVEFROM", model.EFFECTIVEFROM);
                cmd.Parameters.AddWithValue("@PTRATE_EFFECTIVETO", model.EFFECTIVETO);
                cmd.Parameters.AddWithValue("@BBR_ID", Int32.Parse(model.BBR_ID));
                cmd.Parameters.AddWithValue("@ACCTYPE_ID", Int32.Parse(model.ACCTYPE_ID));
                cmd.Parameters.AddWithValue("@ACCPUR_ID", Int32.Parse(model.ACCPUR_ID));
                cmd.Parameters.AddWithValue("@PTRATE_PERCENT", model.PTRATE_PERCENT);
                cmd.Parameters.AddWithValue("@BANK_ID", Int32.Parse(model.BANK_ID));
                cmd.Parameters.AddWithValue("@PTRATE_COMPID", model.PTRATE_COMPID);
                cmd.Parameters.AddWithValue("@PTRATE_STATUS", model.PTRATE_STATUS);
                cmd.Parameters.AddWithValue("@SLAB_ID", Int32.Parse(model.SLAB_ID));
                
                cmd.Parameters.AddWithValue("@sUser", user_session);
                s = cmd.ExecuteNonQuery();
                con.Close();
            }

            string message1 = "Successful";
            string message2 = "unsuccessful";
            if (s == 0)
            {
                return Json(message2, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(message1, JsonRequestBehavior.AllowGet);
            }
            }
            else
            {
                string message_date = "provide correct date ranges";
                return Json(message_date, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult ExportData_profitrate_setup()
        {

            string constr = ConfigurationManager.ConnectionStrings["DAP_NEW"].ConnectionString;
            DataTable table = new DataTable();
            using (SqlConnection con = new SqlConnection(constr))
            {
                //string query = "SELECT * FROM CONFIG_ACCOUNTPURPOSE ";
                using (SqlCommand cmd = new SqlCommand("SP_GET_STP_PROFITRATE", con))
                {
                    SqlDataAdapter ds = new SqlDataAdapter(cmd);
                    ds.Fill(table);
                }
            }
            List<ProfitRate_Setup_pdf> pr_setup_list = new List<ProfitRate_Setup_pdf>();
            for (int i = 0; i < table.Rows.Count; i++)
            {
                pr_setup_list.Add(new ProfitRate_Setup_pdf()
                {
                    Serial = table.Rows[i]["PTRATE_ID"].ToString(),                  
                    SlabName = table.Rows[i]["SLAB_NAME"].ToString(),                   
                    BankName = table.Rows[i]["BANK_NAME"].ToString(),                   
                    BranchName = table.Rows[i]["BBR_NAME"].ToString(),                   
                    AccountTypeName = table.Rows[i]["ACCTYPE_NAME"].ToString(),
                    Percent = table.Rows[i]["PTRATE_PERCENT"].ToString(),
                    EffectiveFrom = table.Rows[i]["PTRATE_EFFECTIVEFROM"].ToString(),
                    EffectiveTo = table.Rows[i]["PTRATE_EFFECTIVETO"].ToString(),
                    Status = table.Rows[i]["PTRATE_STATUS"].ToString(),                 
                    AccountPurposeName = table.Rows[i]["ACCPUR_NAME"].ToString(),                 
                    CompanyName = table.Rows[i]["FUND_NAME"].ToString(),                  
                    CreatedBy = table.Rows[i]["ptrate_createdby"].ToString(),
                    CreatedOn = table.Rows[i]["PTRATE_CREATEDON"].ToString(),
                    ModifiedBy = table.Rows[i]["PTRATE_MODIFIEDBY"].ToString(),
                    ModifiedOn = table.Rows[i]["PTRATE_MODIFIEDON"].ToString(),                
                });
            }
            GridView gv = new GridView();
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "attachment;filename=profitrate_setup_Report.pdf");
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            StringWriter sw = new StringWriter();
            HtmlTextWriter hw = new HtmlTextWriter(sw);
            gv.DataSource = pr_setup_list;
            gv.AllowPaging = false;
            gv.DataBind();
            //for (int i = 0; i < gv.Columns.Count; i++)
            //{
            //    //   gv.Columns[0].HeaderText = "Serial";
            //    gv.Columns[i].ItemStyle.Width = 400;
            //    gv.Columns[i].ItemStyle.Wrap = false;
            //}    
            gv.HeaderRow.Style.Add("width", "300%");
            gv.HeaderRow.Style.Add("font-size", "7px");
            gv.Style.Add("text-decoration", "none");
            gv.Style.Add("font-family", "Arial, Helvetica, sans-serif;");
            gv.Style.Add("font-size", "7px");
            //gv.Style.Add("width", "300%");         
            gv.RenderControl(hw);
            StringReader sr = new StringReader(sw.ToString());
            Document pdfDoc = new Document(PageSize.A4.Rotate(), 50f, 50f, 50f, 50f);
            HTMLWorker htmlparser = new HTMLWorker(pdfDoc);
            PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
            pdfDoc.Open();
            htmlparser.Parse(sr);//this is the error line
            pdfDoc.Close();
            Response.Write(pdfDoc);
            Response.End();
            return RedirectToAction("profitrate_setup", "Home");
        }
        public ActionResult trandetail_setup()
        {
            if (this.HttpContext.Session["username_session"] != null)
    {




              string user_session = this.HttpContext.Session["username_session"].ToString();
              string bnkid = this.HttpContext.Session["bankid_session"].ToString();
              int bkid = Int32.Parse(bnkid);
              string opt_session = this.HttpContext.Session["option_session"].ToString();
              string sp_name = "";

              if (opt_session == "Bank") 
              {
                  sp_name = "SP_GET_TRANDETAIL_BANK";

              }


              if (this.HttpContext.Session["fundid_session"].ToString() != string.Empty)
              {

                  string fid = this.HttpContext.Session["fundid_session"].ToString();
                  int s = Int32.Parse(fid);

                  string constr = ConfigurationManager.ConnectionStrings["DAP_NEW"].ConnectionString;
                  DataTable table = new DataTable();
                  using (SqlConnection con = new SqlConnection(constr))
                  {
                      //string query = "SELECT * FROM CONFIG_ACCOUNTPURPOSE ";
                  using (SqlCommand cmd = new SqlCommand(sp_name, con))   //replace the stored procedure name with string name defined above
                      {
                          var dateAndTime = DateTime.Now;
                          var vdate = this.HttpContext.Session["VALUE_DATE"].ToString();                   //"2018-06-20";
                          var postdate = this.HttpContext.Session["POST_DATE"].ToString();                //"2018-06-21" ;
                          //int s = 1;
                          cmd.CommandType = CommandType.StoredProcedure;
                          //cmd.Parameters.AddWithValue("@Rating_Id", model.rating_id);
                          //cmd.Parameters.AddWithValue("@PTRATE_ID", Int32.Parse(model.PTRATE_ID));
                          if (sp_name == "SP_GET_TRANDETAIL_BANK") 
                          {
                              cmd.Parameters.AddWithValue("@bankid", bkid);
                          } 
                      else
                      {
                      cmd.Parameters.AddWithValue("@fundid", s);
                      }
                      cmd.Parameters.AddWithValue("@valuDate", vdate);
                          cmd.Parameters.AddWithValue("@postDate", postdate);


                          SqlDataAdapter ds = new SqlDataAdapter(cmd);
                          ds.Fill(table);
                      }
                  }

                  List<Trandetail> trandetail_setup_list = new List<Trandetail>();
                  for (int i = 0; i < table.Rows.Count; i++)
                  {
                      trandetail_setup_list.Add(new Trandetail()
                      {
                          txnd_id = table.Rows[i]["txnd_id"].ToString(),
                          txnmstr_id = table.Rows[i]["txnmstr_id"].ToString(),
                          fbacc_id = table.Rows[i]["fbacc_id"].ToString(),
                          //FUND_ID = table.Rows[i]["FUND_ID"].ToString(),
                          //FUND_NAME = table.Rows[i]["FUND_NAME"].ToString(),
                          fund_id = table.Rows[i]["fund_id"].ToString(),
                          fund_name = table.Rows[i]["fund_name"].ToString(),
                          bank_name = table.Rows[i]["bank_name"].ToString(),
                          bbr_name = table.Rows[i]["bbr_name"].ToString(),
                          fbacc_number = table.Rows[i]["fbacc_number"].ToString(),
                          //txnd_type = table.Rows[i]["txnd_type"].ToString(),
                          txnd_amount = table.Rows[i]["txnd_amount"].ToString(),
                          ptrate_percent = table.Rows[i]["ptrate_percent"].ToString(),
                          profit = table.Rows[i]["profit"].ToString(),

                          txnd_l_amount = table.Rows[i]["txnd_l_amount"].ToString(),
                          last_valueDate = Convert.ToDateTime(table.Rows[i]["last_ValueDate"]).ToString("yyyy-MM-dd"),
                          value_date = Convert.ToDateTime(table.Rows[i]["ValueDate"]).ToString("yyyy-MM-dd"),
                          postDate = Convert.ToDateTime(table.Rows[i]["PostDate"]).ToString("yyyy-MM-dd"),


                          // value_date = table.Rows[i]["ValueDate"].ToString(),

                          txnd_status = table.Rows[i]["txnd_status"].ToString(),
                          txnd_postedon = table.Rows[i]["txnd_postedon"].ToString()
                          //PTRATE_COMPID = table.Rows[i]["PTRATE_COMPID"].ToString(),
                          //COMP_NAME=table.Rows[i]["FUND_NAME"].ToString()
                          // ACCPUR_ID = table.Rows[i]["ACCPUR_ID"].ToString(),
                      });

                  }
                  Trandetail user1 = new Trandetail();
                  user1.trandetail_list = trandetail_setup_list;

                  int s2 = 200;
                  string constr2 = ConfigurationManager.ConnectionStrings["DAP_NEW"].ConnectionString;

                  for (int i = 0; i < table.Rows.Count; i++)
                  {

                      using (SqlConnection con = new SqlConnection(constr2))
                      {

                          con.Open();
                          // MessageBox.Show ("Connection Open ! ");
                          // cnn.Close();
                          SqlCommand cmd = new SqlCommand("SP_INSERT_Tran_Detail_MSTR", con);    // to be corrected 
                          // SqlCommand cmd1 = new SqlCommand("select top 1 * from Customer_IDs where ID_NO ='" + txtCnicNo + "'", oCnn);   
                          cmd.CommandType = CommandType.StoredProcedure;
                          cmd.Parameters.AddWithValue("@TXNMSTR_VALDATE", Convert.ToDateTime(table.Rows[i]["ValueDate"]).ToString("yyyy-MM-dd"));

                          cmd.Parameters.AddWithValue("@TXNMSTR_POSTDATE", DateTime.Now.Date);
                          cmd.Parameters.AddWithValue("@FBACC_ID", Int32.Parse(table.Rows[i]["fbacc_id"].ToString()));
                          cmd.Parameters.AddWithValue("@PTRATE_PERCENT", table.Rows[i]["ptrate_percent"].ToString());
                          cmd.Parameters.AddWithValue("@TXND_AMOUNT", table.Rows[i]["txnd_amount"].ToString());
                          cmd.Parameters.AddWithValue("@TXND_ProfitAMOUNT", table.Rows[i]["profit"].ToString());
                          cmd.Parameters.AddWithValue("@sUser", user_session);
                          s2 = cmd.ExecuteNonQuery();
                          con.Close();
                         
                      }
                  }

                  ///////////////////////////////////// call again get function  ///////////////////////////////

                  string fid12 = this.HttpContext.Session["fundid_session"].ToString();
                  int s12 = Int32.Parse(fid12);

                 // string constr = ConfigurationManager.ConnectionStrings["DAP_NEW"].ConnectionString;
                  DataTable table12 = new DataTable();
                  using (SqlConnection con = new SqlConnection(constr))
                  {
                      //string query = "SELECT * FROM CONFIG_ACCOUNTPURPOSE ";
                      using (SqlCommand cmd = new SqlCommand(sp_name, con))
                      {
                          var dateAndTime = DateTime.Now;
                          var vdate = this.HttpContext.Session["VALUE_DATE"].ToString();                   //"2018-06-20";
                          var postdate = this.HttpContext.Session["POST_DATE"].ToString();                //"2018-06-21" ;
                          //int s = 1;
                          cmd.CommandType = CommandType.StoredProcedure;
                          //cmd.Parameters.AddWithValue("@Rating_Id", model.rating_id);
                          //cmd.Parameters.AddWithValue("@PTRATE_ID", Int32.Parse(model.PTRATE_ID));
                          if (sp_name == "SP_GET_TRANDETAIL_BANK")
                          {
                              cmd.Parameters.AddWithValue("@bankid", bkid);
                          }
                          else
                          {
                              cmd.Parameters.AddWithValue("@fundid", s12);
                          }   
                          cmd.Parameters.AddWithValue("@valuDate", vdate);
                          cmd.Parameters.AddWithValue("@postDate", postdate);
                          SqlDataAdapter ds = new SqlDataAdapter(cmd);
                          ds.Fill(table12);
                      }
                  }

                  List<Trandetail> trandetail_setup_list12 = new List<Trandetail>();
                  for (int i = 0; i < table12.Rows.Count; i++)
                  {
                      trandetail_setup_list12.Add(new Trandetail()
                      {
                          txnd_id = table12.Rows[i]["txnd_id"].ToString(),
                          txnmstr_id = table12.Rows[i]["txnmstr_id"].ToString(),
                          fbacc_id = table12.Rows[i]["fbacc_id"].ToString(),
                          //FUND_ID = table.Rows[i]["FUND_ID"].ToString(),
                          //FUND_NAME = table.Rows[i]["FUND_NAME"].ToString(),
                          fund_id = table12.Rows[i]["fund_id"].ToString(),
                          fund_name = table12.Rows[i]["fund_name"].ToString(),
                          bank_name = table12.Rows[i]["bank_name"].ToString(),
                          bbr_name = table12.Rows[i]["bbr_name"].ToString(),
                          fbacc_number = table12.Rows[i]["fbacc_number"].ToString(),
                          //txnd_type = table.Rows[i]["txnd_type"].ToString(),
                          txnd_amount = table12.Rows[i]["txnd_amount"].ToString(),
                          ptrate_percent = table12.Rows[i]["ptrate_percent"].ToString(),
                          profit = table12.Rows[i]["profit"].ToString(),

                          txnd_l_amount = table12.Rows[i]["txnd_l_amount"].ToString(),
                          last_valueDate = Convert.ToDateTime(table12.Rows[i]["last_ValueDate"]).ToString("yyyy-MM-dd"),
                          value_date = Convert.ToDateTime(table12.Rows[i]["ValueDate"]).ToString("yyyy-MM-dd"),
                          postDate = Convert.ToDateTime(table12.Rows[i]["PostDate"]).ToString("yyyy-MM-dd"),


                          // value_date = table.Rows[i]["ValueDate"].ToString(),

                          txnd_status = table12.Rows[i]["txnd_status"].ToString(),
                          txnd_postedon = table12.Rows[i]["txnd_postedon"].ToString()
                          //PTRATE_COMPID = table.Rows[i]["PTRATE_COMPID"].ToString(),
                          //COMP_NAME=table.Rows[i]["FUND_NAME"].ToString()
                          // ACCPUR_ID = table.Rows[i]["ACCPUR_ID"].ToString(),
                      });

                  }
                  Trandetail user12 = new Trandetail();
                  user12.trandetail_list = trandetail_setup_list12;
                  return View(user12);

              }
              else
              {
                  return RedirectToAction("trandetail_fund_setup", "Home");
              }
    }
            else
            {
                return RedirectToAction("login", "Home");
            }
                        
            }
        public JsonResult edit_trandetail(Trandetail model)
        {
            
            int s = 100;
            string user_session = this.HttpContext.Session["username_session"].ToString();
            string constr = ConfigurationManager.ConnectionStrings["DAP_NEW"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                con.Open();
                // MessageBox.Show ("Connection Open ! ");
                // cnn.Close();
              //  SqlCommand cmd = new SqlCommand("SP_INSERT_Tran_Detail_MSTR", con);    // to be corrected 
                // SqlCommand cmd1 = new SqlCommand("select top 1 * from Customer_IDs where ID_NO ='" + txtCnicNo + "'", oCnn);   

                SqlCommand cmd = new SqlCommand("SP_INSERT_Tran_Detail_MSTR_BalanceTransfer", con);    // to be corrected                 
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@TXNMSTR_VALDATE", model.value_date);
                cmd.Parameters.AddWithValue("@TXNMSTR_POSTDATE", DateTime.Now.Date);
                cmd.Parameters.AddWithValue("@FBACC_ID", Int32.Parse(model.fbacc_id));
               // cmd.Parameters.AddWithValue("@PTRATE_PERCENT", "0.0");
                cmd.Parameters.AddWithValue("@TXND_AMOUNT", model.txnd_amount);
               // cmd.Parameters.AddWithValue("@TXND_ProfitAMOUNT", "");
                //if (model.FBACC_ISCLOSED == "TRUE")
                //{
                //    cmd.Parameters.AddWithValue("@FBACC_CLOSINGDATE", model.Closing_Date);
                //}
                //else
                //{
                //    cmd.Parameters.AddWithValue("@FBACC_CLOSINGDATE", DBNull.Value);
                //}

                cmd.Parameters.AddWithValue("@sUser", user_session);
                s = cmd.ExecuteNonQuery();
                con.Close();
            }

            string message1 = "Successful";
            string message2 = "unsuccessful";
            if (s == 0)
            {
                return Json(message2, JsonRequestBehavior.AllowGet);
            }
            else
            {
                this.HttpContext.Session["grid_date"] = model.value_date;

                return Json(message1, JsonRequestBehavior.AllowGet);
            }

        }
        public ActionResult trandetail_fund_setup()
        {
             if (this.HttpContext.Session["username_session"] != null)
    {

        if (this.HttpContext.Session["message successfull"]==null)
        {
            ViewBag.Message = null;
        }
        else { ViewBag.Message = this.HttpContext.Session["message successfull"].ToString(); }

        this.HttpContext.Session["message successfull"] = null;
            
            string constr = ConfigurationManager.ConnectionStrings["DAP_NEW"].ConnectionString;
            DataTable table1 = new DataTable();
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = "select * from STP_FUND";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    SqlDataAdapter ds = new SqlDataAdapter(cmd);
                    ds.Fill(table1);
                }
            }
            List<trandetail_fund> fund_list = new List<trandetail_fund>();

            for (int i = 0; i < table1.Rows.Count; i++)
            {
                //  bankname="Anubhav",shortname="Chaudhary",rating="AA+"

                fund_list.Add(new trandetail_fund()
                {
                    FUND_ID = table1.Rows[i]["FUND_ID"].ToString(),
                    FUND_NAME = table1.Rows[i]["FUND_NAME"].ToString(),
                    //FBACC_COMPID = table1.Rows[i]["FUND_COMPID"].ToString()
                post_date_string = Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd"),
                
                });
            }
            DataTable table2 = new DataTable();
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = "select * from STP_BANK";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    SqlDataAdapter ds = new SqlDataAdapter(cmd);
                    ds.Fill(table2);
                }
            }
            List<trandetail_fund> bank_list = new List<trandetail_fund>();
            for (int i = 0; i < table2.Rows.Count; i++)
            {
                //  bankname="Anubhav",shortname="Chaudhary",rating="AA+"

                bank_list.Add(new trandetail_fund()
                {
                    BANK_ID = table2.Rows[i]["BANK_ID"].ToString(),
                    BANK_NAME = table2.Rows[i]["BANK_NAME"].ToString(),
                    //FBACC_COMPID = table1.Rows[i]["FUND_COMPID"].ToString()
                   // post_date_string = Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd"),

                });
            }

            List<trandetail_fund> trandetail_setup_list12 = new List<trandetail_fund>();
            List<trandetail_fund> trandetail_setup_list = new List<trandetail_fund>();
            if (this.HttpContext.Session["grid_date"] != null)
            {
                string gridate = this.HttpContext.Session["grid_date"].ToString();

                this.HttpContext.Session["grid_date"] = null;

                DataTable table = new DataTable();
                using (SqlConnection con = new SqlConnection(constr))
                {
                    //string query = "SELECT * FROM CONFIG_ACCOUNTPURPOSE ";
                    using (SqlCommand cmd = new SqlCommand("SP_GET_TRANDETAIL_ALL", con))   //replace the stored procedure name with string name defined above
                    {
                        var dateAndTime = DateTime.Now;
                        var valdate = gridate;   // this.HttpContext.Session["VALUE_DATE"].ToString();                   //"2018-06-20";
                        var postdate = this.HttpContext.Session["POST_DATE"].ToString();                //"2018-06-21" ;
                        //int s = 1;
                        cmd.CommandType = CommandType.StoredProcedure;
                        //if (sp_name == "SP_GET_TRANDETAIL_BANK")
                        //{
                        //    cmd.Parameters.AddWithValue("@bankid", bkid);
                        //}
                        //if (sp_name == "SP_GET_TRANDETAIL")
                        //{
                        //    cmd.Parameters.AddWithValue("@fundid", s);
                        //}

                        cmd.Parameters.AddWithValue("@valuDate", valdate);
                        cmd.Parameters.AddWithValue("@postDate", postdate);
                        SqlDataAdapter ds = new SqlDataAdapter(cmd);
                        ds.Fill(table);
                    }
                }

                
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    trandetail_setup_list12.Add(new trandetail_fund()
                    {
                        tf_txnd_id = table.Rows[i]["txnd_id"].ToString(),
                        tf_txnmstr_id = table.Rows[i]["txnmstr_id"].ToString(),
                        tf_fbacc_id = table.Rows[i]["fbacc_id"].ToString(),
                        //FUND_ID = table.Rows[i]["FUND_ID"].ToString(),
                        //FUND_NAME = table.Rows[i]["FUND_NAME"].ToString(),
                        tf_fund_id = table.Rows[i]["fund_id"].ToString(),
                        tf_fund_name = table.Rows[i]["fund_name"].ToString(),
                        tf_bank_name = table.Rows[i]["bank_name"].ToString(),
                        tf_bbr_name = table.Rows[i]["bbr_name"].ToString(),
                        tf_fbacc_number = table.Rows[i]["fbacc_number"].ToString(),
                        //txnd_type = table.Rows[i]["txnd_type"].ToString(),
                        tf_txnd_amount = table.Rows[i]["txnd_amount"].ToString(),
                        tf_ptrate_percent = table.Rows[i]["ptrate_percent"].ToString(),
                        tf_profit = table.Rows[i]["profit"].ToString(),

                        tf_txnd_l_amount = table.Rows[i]["txnd_l_amount"].ToString(),
                        //   tf_last_valueDate = Convert.ToDateTime(table.Rows[i]["last_ValueDate"]).ToString("yyyy-MM-dd"),
                        tf_value_date = Convert.ToDateTime(table.Rows[i]["TXNMSTR_VALDATE"]).ToString("yyyy-MM-dd"),
                        tf_postDate = Convert.ToDateTime(table.Rows[i]["TXNMSTR_POSTDATE"]).ToString("yyyy-MM-dd"),


                        // value_date = table.Rows[i]["ValueDate"].ToString(),

                        tf_txnd_status = table.Rows[i]["txnd_status"].ToString(),
                        tf_txnd_postedon = table.Rows[i]["txnd_postedon"].ToString()
                        //PTRATE_COMPID = table.Rows[i]["PTRATE_COMPID"].ToString(),
                        //COMP_NAME=table.Rows[i]["FUND_NAME"].ToString()
                        // ACCPUR_ID = table.Rows[i]["ACCPUR_ID"].ToString(),
                    });

                }
            
            }
                 

                 //for (int i = 0; i <1; i++)
            //{
            //    trandetail_setup_list12.Add(new Trandetail()
            //    {
            //        txnd_id = "",//table12.Rows[i]["txnd_id"].ToString(),
            //        txnmstr_id = "",//table12.Rows[i]["txnmstr_id"].ToString(),
            //        fbacc_id ="", //table12.Rows[i]["fbacc_id"].ToString(),
            //        //FUND_ID = table.Rows[i]["FUND_ID"].ToString(),
            //        //FUND_NAME = table.Rows[i]["FUND_NAME"].ToString(),
            //        fund_id ="", //table12.Rows[i]["fund_id"].ToString(),
            //        fund_name ="", //table12.Rows[i]["fund_name"].ToString(),
            //        bank_name ="", //table12.Rows[i]["bank_name"].ToString(),
            //        bbr_name ="", //table12.Rows[i]["bbr_name"].ToString(),
            //        fbacc_number ="", //table12.Rows[i]["fbacc_number"].ToString(),
            //        //txnd_type = table.Rows[i]["txnd_type"].ToString(),
            //        txnd_amount ="", //table12.Rows[i]["txnd_amount"].ToString(),
            //        ptrate_percent ="", //table12.Rows[i]["ptrate_percent"].ToString(),
            //        profit ="", //table12.Rows[i]["profit"].ToString(),

            //        txnd_l_amount = "",//table12.Rows[i]["txnd_l_amount"].ToString(),
            //        last_valueDate ="", //Convert.ToDateTime(table12.Rows[i]["last_ValueDate"]).ToString("yyyy-MM-dd"),
            //        value_date = "", //Convert.ToDateTime(table12.Rows[i]["ValueDate"]).ToString("yyyy-MM-dd"),
            //        postDate = "",  // Convert.ToDateTime(table12.Rows[i]["PostDate"]).ToString("yyyy-MM-dd"),
            //        // value_date = table.Rows[i]["ValueDate"].ToString(),

            //        txnd_status ="", //table12.Rows[i]["txnd_status"].ToString(),
            //        txnd_postedon = ""//table12.Rows[i]["txnd_postedon"].ToString()
            //        //PTRATE_COMPID = table.Rows[i]["PTRATE_COMPID"].ToString(),
            //        //COMP_NAME=table.Rows[i]["FUND_NAME"].ToString()
            //        // ACCPUR_ID = table.Rows[i]["ACCPUR_ID"].ToString(),
            //    });

            //}
           
            trandetail_fund user1 = new trandetail_fund();
            user1.FUND_OPTION_LIST = fund_list;
            user1.BANK_OPTION_LIST = bank_list;
            user1.trandetail_fund_list = trandetail_setup_list12;
            user1.post_date_string = Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd");
            return View(user1);
    }
             else
             {
                 return RedirectToAction("login", "Home");
             }             
             }
        [HttpPost]
        public ActionResult trandetail_fund_setup(trandetail_fund model) 
        {
           // string OPT = Request.Form["radio_fund"].ToString();
            model.search_option = Request.Form["wise"].ToString();

            if (model.Val_Date > DateTime.Now.Date)
            {
                ModelState.AddModelError("Val_Date", "kindly enter value date less then post date");
            }
            if (ModelState.IsValid)
            {

                string fundid = model.FUND_ID;
                string vdate = Convert.ToDateTime(model.Val_Date).ToString("yyyy-MM-dd");
                string pdate = Convert.ToDateTime(DateTime.Now.Date).ToString("yyyy-MM-dd");
                this.HttpContext.Session["VALUE_DATE"] = vdate;
                this.HttpContext.Session["fundid_session"] = fundid;
                this.HttpContext.Session["POST_DATE"] = pdate;
                this.HttpContext.Session["option_session"] = model.search_option;
                this.HttpContext.Session["bankid_session"] = model.BANK_ID;
                //this.HttpContext.Session["fund_bank_acc_id"] = model.FUND_BANK_ACC_ID;
                ////////////////////////////////////////////////////////////////////////////////////



                string constr = ConfigurationManager.ConnectionStrings["DAP_NEW"].ConnectionString;
                DataTable table1 = new DataTable();
                using (SqlConnection con = new SqlConnection(constr))
                {
                    string query = "select * from STP_FUND";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        SqlDataAdapter ds = new SqlDataAdapter(cmd);
                        ds.Fill(table1);
                    }
                }
                List<trandetail_fund> fund_list = new List<trandetail_fund>();

                for (int i = 0; i < table1.Rows.Count; i++)
                {
                    //  bankname="Anubhav",shortname="Chaudhary",rating="AA+"

                    fund_list.Add(new trandetail_fund()
                    {
                        FUND_ID = table1.Rows[i]["FUND_ID"].ToString(),
                        FUND_NAME = table1.Rows[i]["FUND_NAME"].ToString(),
                        //FBACC_COMPID = table1.Rows[i]["FUND_COMPID"].ToString()
                        post_date_string = Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd"),

                    });
                }

                DataTable table2 = new DataTable();
                using (SqlConnection con = new SqlConnection(constr))
                {
                    string query = "select * from STP_BANK";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        SqlDataAdapter ds = new SqlDataAdapter(cmd);
                        ds.Fill(table2);
                    }
                }
                List<trandetail_fund> bank_list = new List<trandetail_fund>();
                for (int i = 0; i < table2.Rows.Count; i++)
                {
                    //  bankname="Anubhav",shortname="Chaudhary",rating="AA+"

                    bank_list.Add(new trandetail_fund()
                    {
                        BANK_ID = table2.Rows[i]["BANK_ID"].ToString(),
                        BANK_NAME = table2.Rows[i]["BANK_NAME"].ToString(),
                        //FBACC_COMPID = table1.Rows[i]["FUND_COMPID"].ToString()
                        // post_date_string = Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd"),

                    });
                }

                /////////////////////////////////////////////////////////////////////////////////
                string user_session = this.HttpContext.Session["username_session"].ToString();
                string bnkid = this.HttpContext.Session["bankid_session"].ToString();
                int bkid = Int32.Parse(bnkid);
                string opt_session = this.HttpContext.Session["option_session"].ToString();
                string sp_name = "";

                if (opt_session == "Bank")
                {
                    sp_name = "SP_GET_TRANDETAIL_BANK";

                }
                else if (opt_session == "Fund")
                {
                    sp_name = "SP_GET_TRANDETAIL";
                }
                else if (opt_session == "All")
                {
                    sp_name = "SP_GET_TRANDETAIL_ALL";
                }

                //if (this.HttpContext.Session["fundid_session"].ToString() != string.Empty)
                //{

                string fid = this.HttpContext.Session["fundid_session"].ToString();
                int s = Int32.Parse(fid);

                //  string constr = ConfigurationManager.ConnectionStrings["DAP_NEW"].ConnectionString;
                DataTable table = new DataTable();
                using (SqlConnection con = new SqlConnection(constr))
                {
                    //string query = "SELECT * FROM CONFIG_ACCOUNTPURPOSE ";
                    using (SqlCommand cmd = new SqlCommand(sp_name, con))   //replace the stored procedure name with string name defined above
                    {
                        var dateAndTime = DateTime.Now;
                        var valdate = this.HttpContext.Session["VALUE_DATE"].ToString();                   //"2018-06-20";
                        var postdate = this.HttpContext.Session["POST_DATE"].ToString();                //"2018-06-21" ;
                        //int s = 1;
                        cmd.CommandType = CommandType.StoredProcedure;                       
                        if (sp_name == "SP_GET_TRANDETAIL_BANK")
                        {
                            cmd.Parameters.AddWithValue("@bankid",bkid);
                        }
                        if (sp_name == "SP_GET_TRANDETAIL")
                        {
                            cmd.Parameters.AddWithValue("@fundid",s);
                        }

                        cmd.Parameters.AddWithValue("@valuDate",valdate);
                        cmd.Parameters.AddWithValue("@postDate",postdate);
                        SqlDataAdapter ds = new SqlDataAdapter(cmd);
                        ds.Fill(table);
                    }
                }

                List<trandetail_fund> trandetail_setup_list = new List<trandetail_fund>();
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    trandetail_setup_list.Add(new trandetail_fund()
                    {
                        tf_txnd_id = table.Rows[i]["txnd_id"].ToString(),
                        tf_txnmstr_id = table.Rows[i]["txnmstr_id"].ToString(),
                        tf_fbacc_id = table.Rows[i]["fbacc_id"].ToString(),
                        //FUND_ID = table.Rows[i]["FUND_ID"].ToString(),
                        //FUND_NAME = table.Rows[i]["FUND_NAME"].ToString(),
                        tf_fund_id = table.Rows[i]["fund_id"].ToString(),
                        tf_fund_name = table.Rows[i]["fund_name"].ToString(),
                        tf_bank_name = table.Rows[i]["bank_name"].ToString(),
                        tf_bbr_name = table.Rows[i]["bbr_name"].ToString(),
                        tf_fbacc_number = table.Rows[i]["fbacc_number"].ToString(),
                        //txnd_type = table.Rows[i]["txnd_type"].ToString(),
                        tf_txnd_amount = table.Rows[i]["txnd_amount"].ToString(),
                        tf_ptrate_percent = table.Rows[i]["ptrate_percent"].ToString(),
                        tf_profit = table.Rows[i]["profit"].ToString(),

                        tf_txnd_l_amount = table.Rows[i]["txnd_l_amount"].ToString(),
                     //   tf_last_valueDate = Convert.ToDateTime(table.Rows[i]["last_ValueDate"]).ToString("yyyy-MM-dd"),
                        tf_value_date = Convert.ToDateTime(table.Rows[i]["TXNMSTR_VALDATE"]).ToString("yyyy-MM-dd"),
                        tf_postDate = Convert.ToDateTime(table.Rows[i]["TXNMSTR_POSTDATE"]).ToString("yyyy-MM-dd"),


                        // value_date = table.Rows[i]["ValueDate"].ToString(),

                        tf_txnd_status = table.Rows[i]["txnd_status"].ToString(),
                        tf_txnd_postedon = table.Rows[i]["txnd_postedon"].ToString()
                        //PTRATE_COMPID = table.Rows[i]["PTRATE_COMPID"].ToString(),
                        //COMP_NAME=table.Rows[i]["FUND_NAME"].ToString()
                        // ACCPUR_ID = table.Rows[i]["ACCPUR_ID"].ToString(),
                    });

                }
                //Trandetail user1 = new Trandetail();
                //user1.trandetail_list = trandetail_setup_list;

                int s2 = 200;
                string constr2 = ConfigurationManager.ConnectionStrings["DAP_NEW"].ConnectionString;

              
                
                ////////////  below code was inserting data in txn_master ,when search option was used //////////////
                
                
                //for (int i = 0; i < table.Rows.Count; i++)
                //{

                //    using (SqlConnection con = new SqlConnection(constr2))
                //    {

                //        con.Open();
                //        // MessageBox.Show ("Connection Open ! ");
                //        // cnn.Close();
                //        SqlCommand cmd = new SqlCommand("SP_INSERT_Tran_Detail_MSTR", con);    // to be corrected 
                //        // SqlCommand cmd1 = new SqlCommand("select top 1 * from Customer_IDs where ID_NO ='" + txtCnicNo + "'", oCnn);   
                //        cmd.CommandType = CommandType.StoredProcedure;
                //        cmd.Parameters.AddWithValue("@TXNMSTR_VALDATE", Convert.ToDateTime(table.Rows[i]["ValueDate"]).ToString("yyyy-MM-dd"));

                //        cmd.Parameters.AddWithValue("@TXNMSTR_POSTDATE", DateTime.Now.Date);
                //        cmd.Parameters.AddWithValue("@FBACC_ID", Int32.Parse(table.Rows[i]["fbacc_id"].ToString()));
                //        cmd.Parameters.AddWithValue("@PTRATE_PERCENT", table.Rows[i]["ptrate_percent"].ToString());
                //        cmd.Parameters.AddWithValue("@TXND_AMOUNT", table.Rows[i]["txnd_amount"].ToString());
                //        cmd.Parameters.AddWithValue("@TXND_ProfitAMOUNT", table.Rows[i]["profit"].ToString());
                //        cmd.Parameters.AddWithValue("@sUser", user_session);
                //        s2 = cmd.ExecuteNonQuery();
                //        con.Close();

                //    }
                //}

                ///////////////////////////////////// call again get function  ///////////////////////////////

                string fid12 = this.HttpContext.Session["fundid_session"].ToString();
                int s12 = Int32.Parse(fid12);

                // string constr = ConfigurationManager.ConnectionStrings["DAP_NEW"].ConnectionString;
                DataTable table12 = new DataTable();
                using (SqlConnection con = new SqlConnection(constr))
                {
                    //string query = "SELECT * FROM CONFIG_ACCOUNTPURPOSE ";
                    using (SqlCommand cmd = new SqlCommand(sp_name, con))
                    {
                        var dateAndTime = DateTime.Now;
                        var valdate = this.HttpContext.Session["VALUE_DATE"].ToString();                   //"2018-06-20";
                        var postdate = this.HttpContext.Session["POST_DATE"].ToString();                //"2018-06-21" ;
                        //int s = 1;
                        cmd.CommandType = CommandType.StoredProcedure;
                        //cmd.Parameters.AddWithValue("@Rating_Id", model.rating_id);
                        //cmd.Parameters.AddWithValue("@PTRATE_ID", Int32.Parse(model.PTRATE_ID));
                        if (sp_name == "SP_GET_TRANDETAIL_BANK")
                        {
                            cmd.Parameters.AddWithValue("@bankid", bkid);
                        }
                        if (sp_name == "SP_GET_TRANDETAIL")
                        {
                            cmd.Parameters.AddWithValue("@fundid", s);
                        }
                        cmd.Parameters.AddWithValue("@valuDate", valdate);
                        cmd.Parameters.AddWithValue("@postDate", postdate);
                        SqlDataAdapter ds = new SqlDataAdapter(cmd);
                        ds.Fill(table12);
                    }
                }

                List<trandetail_fund> trandetail_setup_list12 = new List<trandetail_fund>();
                for (int i = 0; i < table12.Rows.Count; i++)
                {
                    trandetail_setup_list12.Add(new trandetail_fund()
                    {
                        tf_txnd_id = table12.Rows[i]["txnd_id"].ToString(),
                        tf_txnmstr_id = table12.Rows[i]["txnmstr_id"].ToString(),
                        tf_fbacc_id = table12.Rows[i]["fbacc_id"].ToString(),
                        //FUND_ID = table.Rows[i]["FUND_ID"].ToString(),
                        //FUND_NAME = table.Rows[i]["FUND_NAME"].ToString(),
                        tf_fund_id = table12.Rows[i]["fund_id"].ToString(),
                        tf_fund_name = table12.Rows[i]["fund_name"].ToString(),
                        tf_bank_name = table12.Rows[i]["bank_name"].ToString(),
                        tf_bbr_name = table12.Rows[i]["bbr_name"].ToString(),
                        tf_fbacc_number = table12.Rows[i]["fbacc_number"].ToString(),
                        //txnd_type = table.Rows[i]["txnd_type"].ToString(),
                        tf_txnd_amount = table12.Rows[i]["txnd_amount"].ToString(),
                        tf_ptrate_percent = table12.Rows[i]["ptrate_percent"].ToString(),
                        tf_profit = table12.Rows[i]["profit"].ToString(),

                        tf_txnd_l_amount = table.Rows[i]["txnd_l_amount"].ToString(),
                        //   tf_last_valueDate = Convert.ToDateTime(table.Rows[i]["last_ValueDate"]).ToString("yyyy-MM-dd"),
                        tf_value_date = Convert.ToDateTime(table.Rows[i]["TXNMSTR_VALDATE"]).ToString("yyyy-MM-dd"),
                        tf_postDate = Convert.ToDateTime(table.Rows[i]["TXNMSTR_POSTDATE"]).ToString("yyyy-MM-dd"),

                        // value_date = table.Rows[i]["ValueDate"].ToString(),

                        tf_txnd_status = table12.Rows[i]["txnd_status"].ToString(),
                        tf_txnd_postedon = table12.Rows[i]["txnd_postedon"].ToString()
                        //PTRATE_COMPID = table.Rows[i]["PTRATE_COMPID"].ToString(),
                        //COMP_NAME=table.Rows[i]["FUND_NAME"].ToString()
                        // ACCPUR_ID = table.Rows[i]["ACCPUR_ID"].ToString(),
                    });

                }
                trandetail_fund user1 = new trandetail_fund();
                user1.FUND_OPTION_LIST = fund_list;
                user1.BANK_OPTION_LIST = bank_list;
                user1.trandetail_fund_list = trandetail_setup_list12;
                user1.post_date_string = Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd");
                return View(user1);      



////////////////////////////////////////////////


//
           //     return RedirectToAction("trandetail_setup", "Home");
                }
                else
                {
                   string constr = ConfigurationManager.ConnectionStrings["DAP_NEW"].ConnectionString;
                    DataTable table19 = new DataTable();
                    using (SqlConnection con = new SqlConnection(constr))
                    {
                        string query = "select * from STP_FUND";
                        using (SqlCommand cmd = new SqlCommand(query, con))
                        {
                            SqlDataAdapter ds = new SqlDataAdapter(cmd);
                            ds.Fill(table19);
                        }
                    }
                    List<trandetail_fund> fund_list19 = new List<trandetail_fund>();

                    for (int i = 0; i < table19.Rows.Count; i++)
                    {
                        //  bankname="Anubhav",shortname="Chaudhary",rating="AA+"

                        fund_list19.Add(new trandetail_fund()
                        {
                            FUND_ID = table19.Rows[i]["FUND_ID"].ToString(),
                            FUND_NAME = table19.Rows[i]["FUND_NAME"].ToString(),
                            //FBACC_COMPID = table1.Rows[i]["FUND_COMPID"].ToString()
                            post_date_string = Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd"),

                        });
                    }

                    DataTable table29 = new DataTable();
                    using (SqlConnection con = new SqlConnection(constr))
                    {
                        string query = "select * from STP_BANK";
                        using (SqlCommand cmd = new SqlCommand(query, con))
                        {
                            SqlDataAdapter ds = new SqlDataAdapter(cmd);
                            ds.Fill(table29);
                        }
                    }
                    List<trandetail_fund> bank_list19 = new List<trandetail_fund>();
                    for (int i = 0; i < table29.Rows.Count; i++)
                    {
                        //  bankname="Anubhav",shortname="Chaudhary",rating="AA+"

                        bank_list19.Add(new trandetail_fund()
                        {
                            BANK_ID = table29.Rows[i]["BANK_ID"].ToString(),
                            BANK_NAME = table29.Rows[i]["BANK_NAME"].ToString(),
                            //FBACC_COMPID = table1.Rows[i]["FUND_COMPID"].ToString()
                            // post_date_string = Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd"),

                        });
                    }

                    List<trandetail_fund> trandetail_setup_list12 = new List<trandetail_fund>();

                    trandetail_fund user1 = new trandetail_fund();
                    user1.FUND_OPTION_LIST = fund_list19;
                    user1.BANK_OPTION_LIST = bank_list19;
                    user1.trandetail_fund_list = trandetail_setup_list12;    
                    user1.post_date_string = Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd");
                    return View(user1);
                }
           // }      
        }
       // [HttpPost]
        public JsonResult bulk_post(Trandetail model) 
        {
            string user_session = this.HttpContext.Session["username_session"].ToString();
            DataTable table = new DataTable();
            if (this.HttpContext.Session["fundid_session"].ToString() != string.Empty)
            {

                string fid = this.HttpContext.Session["fundid_session"].ToString();
                int s = Int32.Parse(fid);

                string constr = ConfigurationManager.ConnectionStrings["DAP_NEW"].ConnectionString;
               // DataTable table = new DataTable();
                using (SqlConnection con = new SqlConnection(constr))
                {
                    //string query = "SELECT * FROM CONFIG_ACCOUNTPURPOSE ";
                    using (SqlCommand cmd = new SqlCommand("SP_GET_TRANDETAIL", con))
                    {
                        var dateAndTime = DateTime.Now;
                        var vdate = this.HttpContext.Session["VALUE_DATE"].ToString();                   //"2018-06-20";
                        var postdate = this.HttpContext.Session["POST_DATE"].ToString();                //"2018-06-21" ;
                        //int s = 1;
                        cmd.CommandType = CommandType.StoredProcedure;
                        //cmd.Parameters.AddWithValue("@Rating_Id", model.rating_id);
                        //cmd.Parameters.AddWithValue("@PTRATE_ID", Int32.Parse(model.PTRATE_ID));
                        cmd.Parameters.AddWithValue("@fundid", s);
                        cmd.Parameters.AddWithValue("@valuDate", vdate);
                        cmd.Parameters.AddWithValue("@postDate", postdate);


                        SqlDataAdapter ds = new SqlDataAdapter(cmd);
                        ds.Fill(table);
                    }
                }

                List<Trandetail> trandetail_setup_list = new List<Trandetail>();
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    trandetail_setup_list.Add(new Trandetail()
                    {
                        txnd_id = table.Rows[i]["txnd_id"].ToString(),
                        txnmstr_id = table.Rows[i]["txnmstr_id"].ToString(),
                        fbacc_id = table.Rows[i]["fbacc_id"].ToString(),
                        //FUND_ID = table.Rows[i]["FUND_ID"].ToString(),
                        //FUND_NAME = table.Rows[i]["FUND_NAME"].ToString(),
                        fund_id = table.Rows[i]["fund_id"].ToString(),
                        fund_name = table.Rows[i]["fund_name"].ToString(),
                        bank_name = table.Rows[i]["bank_name"].ToString(),
                        bbr_name = table.Rows[i]["bbr_name"].ToString(),
                        fbacc_number = table.Rows[i]["fbacc_number"].ToString(),
                        //txnd_type = table.Rows[i]["txnd_type"].ToString(),
                        txnd_amount = table.Rows[i]["txnd_amount"].ToString(),
                        ptrate_percent = table.Rows[i]["ptrate_percent"].ToString(),
                        profit = table.Rows[i]["profit"].ToString(),

                        txnd_l_amount = table.Rows[i]["txnd_l_amount"].ToString(),
                        last_valueDate = Convert.ToDateTime(table.Rows[i]["last_ValueDate"]).ToString("yyyy-MM-dd"),
                        value_date = Convert.ToDateTime(table.Rows[i]["ValueDate"]).ToString("yyyy-MM-dd"),
                        postDate = Convert.ToDateTime(table.Rows[i]["PostDate"]).ToString("yyyy-MM-dd"),


                        // value_date = table.Rows[i]["ValueDate"].ToString(),

                        txnd_status = table.Rows[i]["txnd_status"].ToString(),
                        txnd_postedon = table.Rows[i]["txnd_postedon"].ToString()
                        //PTRATE_COMPID = table.Rows[i]["PTRATE_COMPID"].ToString(),
                        //COMP_NAME=table.Rows[i]["FUND_NAME"].ToString()
                        // ACCPUR_ID = table.Rows[i]["ACCPUR_ID"].ToString(),
                    });

                }
                Trandetail user1 = new Trandetail();
                user1.trandetail_list = trandetail_setup_list;
              //  return View(user1);
            }
            
           ///////////////////////////////////// insert trandetail fund /////////////////////////////////////
            int s2 = 200;
            string constr2 = ConfigurationManager.ConnectionStrings["DAP_NEW"].ConnectionString;

            for (int i = 0; i < table.Rows.Count; i++)
            {

                using (SqlConnection con = new SqlConnection(constr2))
                {

                    con.Open();
                    // MessageBox.Show ("Connection Open ! ");
                    // cnn.Close();
                    SqlCommand cmd = new SqlCommand("SP_INSERT_Tran_Detail_MSTR", con);    // to be corrected 
                    // SqlCommand cmd1 = new SqlCommand("select top 1 * from Customer_IDs where ID_NO ='" + txtCnicNo + "'", oCnn);   
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TXNMSTR_VALDATE", Convert.ToDateTime(table.Rows[i]["ValueDate"]).ToString("yyyy-MM-dd"));                    
                    cmd.Parameters.AddWithValue("@TXNMSTR_POSTDATE", DateTime.Now.Date);
                    cmd.Parameters.AddWithValue("@FBACC_ID", Int32.Parse(table.Rows[i]["fbacc_id"].ToString()));
                    cmd.Parameters.AddWithValue("@PTRATE_PERCENT", table.Rows[i]["ptrate_percent"].ToString());
                    cmd.Parameters.AddWithValue("@TXND_AMOUNT", table.Rows[i]["txnd_amount"].ToString());
                    cmd.Parameters.AddWithValue("@TXND_ProfitAMOUNT", table.Rows[i]["profit"].ToString());


                    //if (model.FBACC_ISCLOSED == "TRUE")
                    //{
                    //    cmd.Parameters.AddWithValue("@FBACC_CLOSINGDATE", model.Closing_Date);
                    //}
                    //else
                    //{
                    //    cmd.Parameters.AddWithValue("@FBACC_CLOSINGDATE", DBNull.Value);
                    //}

                    cmd.Parameters.AddWithValue("@sUser", user_session);
                    s2 = cmd.ExecuteNonQuery();
                    con.Close();
                }


            }


                        
            ////////////////////////////////// query work ahead  //////////////////////////////////////
            
            int s1 = 100;
            int b = 100;
            string vdate1 = this.HttpContext.Session["VALUE_DATE"].ToString();
            string FUNDID=this.HttpContext.Session["fundid_session"].ToString();
          //  string user_session = this.HttpContext.Session["username_session"].ToString();
            string constr1 = ConfigurationManager.ConnectionStrings["DAP_NEW"].ConnectionString;
            using (SqlConnection con =
             new SqlConnection(constr1))
            {
                con.Open();
                using (SqlCommand cmd =
                    new SqlCommand("UPDATE TXN_TRANMASTER SET TXNMSTR_STATUS=@STATUS,TXNMSTR_MODIFIEDON=@MODIFIED_DATE" +
                        " WHERE TXNMSTR_VALDATE=@VDATE "+
                        " AND FBACC_ID in (select FBACC_ID from STP_FUNDBANKACCOUNT where FUND_ID=@FID)", con))
                {
                    cmd.Parameters.AddWithValue("@STATUS","Post");
                    cmd.Parameters.AddWithValue("@VDATE",vdate1);
                    cmd.Parameters.AddWithValue("@MODIFIED_DATE",DateTime.Now.ToString() );
                    cmd.Parameters.AddWithValue("@FID", FUNDID);

                    s1 = cmd.ExecuteNonQuery();
                    con.Close();
                    //rows number of record got updated
                }
            }

            using (SqlConnection con =
            new SqlConnection(constr1))
            {
                con.Open();
                using (SqlCommand cmd1 =
            new SqlCommand("UPDATE TXN_TRANDETAIL SET TXND_STATUS=@STATUS, TXND_MODIFIEDON=@MODIFIED_DATE " +
           " where TXND_STATUS='unpost' and TXNMSTR_ID in  (select TXNMSTR_ID from TXN_TRANMASTER where TXNMSTR_VALDATE=@VDATE " +
            "AND FBACC_ID in (select FBACC_ID from STP_FUNDBANKACCOUNT where FUND_ID=@FID))", con))
                {
                    cmd1.Parameters.AddWithValue("@STATUS","Post");
                    cmd1.Parameters.AddWithValue("@VDATE",vdate1);
                    // cmd.Parameters.AddWithValue("@Address", );
                    cmd1.Parameters.AddWithValue("@MODIFIED_DATE", DateTime.Now.ToString());
                    cmd1.Parameters.AddWithValue("@FID", FUNDID);
                    b = cmd1.ExecuteNonQuery();
                    con.Close();
                    //rows number of record got updated
                }
            }
            string message1 = "Successful";
            string message2 = "unsuccessful";
            if (s1 == 0 || b==0 ||s2==0)
            {
                return Json(message2, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(message1, JsonRequestBehavior.AllowGet);
            }
            
            
            
           // return View();        
        }
        [HttpPost]
        public ActionResult bulk_update(trandetail_fund model)
        {
            return View();
        }  // work needs to be done 
        public ActionResult Country()
        {
              if (this.HttpContext.Session["username_session"] != null)
    {



            DataTable table = new DataTable();
            string constr = ConfigurationManager.ConnectionStrings["DAP_NEW"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                //string query="select RATING_ID,RATING_NAME,RATING_SHORTNAME,RATING_STATUS,RATING_ISDEFAULT from config_rating ";
                using (SqlCommand cmd = new SqlCommand("SP_GET_REGION_COUNTRY", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    con.Open();
                    SqlDataAdapter ds = new SqlDataAdapter(cmd);
                    ds.Fill(table);
                    con.Close();
                }
            }

            List<Country> c_list  = new List<Country>();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                c_list.Add(new Country()
                {
                    id = table.Rows[i]["COUNTRY_ID"].ToString(),
                    name = table.Rows[i]["COUNTRY_OFFICIALNAME"].ToString(),
                    sname = table.Rows[i]["COUNTRY_SHORTNAME"].ToString(),
                    COUNTRY_STATUS = table.Rows[i]["COUNTRY_STATUS"].ToString(),

                });
            }
            Country user1 = new Country();
            user1.country_list = c_list;
            return View(user1);

    }
              else
              {
                  return RedirectToAction("login", "Home");

              }


        }
        [HttpPost]
        public ActionResult Country(Country model) 
        {
            if (string.IsNullOrEmpty(model.name))
            {
               // ModelState.AddModelError("Name", "Kindly Provide Name");
            }
            if (string.IsNullOrEmpty(model.sname))
            {
              //  ModelState.AddModelError("Name", "Kindly Provide Name");
            }
            if (string.IsNullOrEmpty(model.COUNTRY_STATUS))
            { 
            
            }
            if (ModelState.IsValid)
            {
                int s = 100;
                string user_session = this.HttpContext.Session["username_session"].ToString();
                // string ratingid = model.rating_id.ToString();
                string name = model.name.ToString();
                string sname = model.sname.ToString();

                string constr = ConfigurationManager.ConnectionStrings["DAP_NEW"].ConnectionString;
                using (SqlConnection con = new SqlConnection(constr))
                {

                    con.Open();
                    // MessageBox.Show ("Connection Open ! ");
                    // cnn.Close();
                    SqlCommand cmd = new SqlCommand("SP_INSERT_COUNTRY", con);    // to be corrected 
                    // SqlCommand cmd1 = new SqlCommand("select top 1 * from Customer_IDs where ID_NO ='" + txtCnicNo + "'", oCnn);   
                    cmd.CommandType = CommandType.StoredProcedure;
                    //cmd.Parameters.AddWithValue("@Rating_Id", model.rating_id);
                    cmd.Parameters.AddWithValue("@Country_Name", model.name);
                    cmd.Parameters.AddWithValue("@Country_ShortName", model.sname);
                    cmd.Parameters.AddWithValue("@Country_status", model.COUNTRY_STATUS);
                    cmd.Parameters.AddWithValue("@sUser", user_session);
                    s = cmd.ExecuteNonQuery();
                    con.Close();
                }

                return RedirectToAction("Country", "Home");
            }
            else
            {
                DataTable table = new DataTable();
                string constr = ConfigurationManager.ConnectionStrings["DAP_NEW"].ConnectionString;
                using (SqlConnection con = new SqlConnection(constr))
                {
                    //string query="select RATING_ID,RATING_NAME,RATING_SHORTNAME,RATING_STATUS,RATING_ISDEFAULT from config_rating ";
                    using (SqlCommand cmd = new SqlCommand("SP_GET_REGION_COUNTRY", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        con.Open();
                        SqlDataAdapter ds = new SqlDataAdapter(cmd);
                        ds.Fill(table);
                        con.Close();
                    }
                }
                List<Country> c_list = new List<Country>();
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    c_list.Add(new Country()
                    {
                        id = table.Rows[i]["COUNTRY_ID"].ToString(),
                        name = table.Rows[i]["COUNTRY_OFFICIALNAME"].ToString(),
                        sname = table.Rows[i]["COUNTRY_SHORTNAME"].ToString(),
                    });
                }
                Country user1 = new Country();
                user1.country_list = c_list;
                return View(user1);
               // return RedirectToAction("Country", "Home");
            }        
        }
        public JsonResult edit_Country(Country model)
        {
            int s = 100;
            string user_session = this.HttpContext.Session["username_session"].ToString();
            string ratingid = model.id.ToString();
            string ratingname = model.name.ToString();
            string ratingshortname = model.sname.ToString();
            
            
            string constr = ConfigurationManager.ConnectionStrings["DAP_NEW"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                con.Open();
                // MessageBox.Show ("Connection Open ! ");
                // cnn.Close();
                SqlCommand cmd = new SqlCommand("SP_UPDATE_COUNTRY", con);    // to be corrected 
                // SqlCommand cmd1 = new SqlCommand("select top 1 * from Customer_IDs where ID_NO ='" + txtCnicNo + "'", oCnn);   
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", model.id);
                cmd.Parameters.AddWithValue("@Name", model.name);
                cmd.Parameters.AddWithValue("@ShortName", model.sname);
                cmd.Parameters.AddWithValue("@Country_Status",model.COUNTRY_STATUS);
                //cmd.Parameters.AddWithValue("@Rating_IsDefault", ratingisdefault);
                cmd.Parameters.AddWithValue("@sMUser", user_session);
                s = cmd.ExecuteNonQuery();
                con.Close();
            }
            string message1 = "Successful";
            string message2 = "unsuccessful";
            if (s == 0)
            {
                //TempData["Success"] = "Un Successfull";
                //return RedirectToAction("Rating", "Home");                
                return Json(message2, JsonRequestBehavior.AllowGet);
            }
            else
            {
                //TempData["Success"] = "Edit Successfully!";
                //return RedirectToAction("Rating", "Home");
                return Json(message1, JsonRequestBehavior.AllowGet);
            }
            //return View();
        }
        public ActionResult State()
        {
 if (this.HttpContext.Session["username_session"] != null)
    {



            DataTable table = new DataTable();
            string constr = ConfigurationManager.ConnectionStrings["DAP_NEW"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                //string query="select RATING_ID,RATING_NAME,RATING_SHORTNAME,RATING_STATUS,RATING_ISDEFAULT from config_rating ";
                using (SqlCommand cmd = new SqlCommand("SP_GET_REGION_STATE", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    con.Open();
                    SqlDataAdapter ds = new SqlDataAdapter(cmd);
                    ds.Fill(table);
                    con.Close();
                }
            }

            List<State> s_list = new List<State>();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                s_list.Add(new State()
                {
                    id = table.Rows[i]["STATE_ID"].ToString(),
                    name = table.Rows[i]["STATE_NAME"].ToString(),
                    sname = table.Rows[i]["STATE_SHORTNAME"].ToString(),
                    STATE_STATUS = table.Rows[i]["STATE_STATUS"].ToString(),                    
                    country_id = table.Rows[i]["COUNTRY_ID"].ToString(),
                    country_name = table.Rows[i]["COUNTRY_OFFICIALNAME"].ToString(),
                });
            }

            DataTable table1 = new DataTable();
            using (SqlConnection con = new SqlConnection(constr))
            {
                //string query="select RATING_ID,RATING_NAME,RATING_SHORTNAME,RATING_STATUS,RATING_ISDEFAULT from config_rating ";
                using (SqlCommand cmd = new SqlCommand("SP_GET_REGION_COUNTRY", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    con.Open();
                    SqlDataAdapter ds = new SqlDataAdapter(cmd);
                    ds.Fill(table1);
                    con.Close();
                }
            }

            List<State> c_list = new List<State>();

            for (int i = 0; i < table1.Rows.Count; i++)
            {
                c_list.Add(new State()
                {

                    country_id = table1.Rows[i]["COUNTRY_ID"].ToString(),
                    country_name = table1.Rows[i]["COUNTRY_OFFICIALNAME"].ToString(),

                });
            }
            State user1 = new State();
            user1.state_list = s_list;
            user1.country_list = c_list;
            return View(user1);
    }
 else
 {
     return RedirectToAction("login", "Home");

 }
 }
        [HttpPost]
        public ActionResult State(State model)
        {
            if (string.IsNullOrEmpty(model.name))
            {
              //  ModelState.AddModelError("Name", "Kindly Provide Name");
            }
            if (string.IsNullOrEmpty(model.sname))
            {
              //  ModelState.AddModelError("SName", "Kindly Provide Name");
            }
            if (string.IsNullOrEmpty(model.STATE_STATUS))
            {
                //  ModelState.AddModelError("SName", "Kindly Provide Name");
            }
            if (string.IsNullOrEmpty(model.country_id))
            {
                ModelState.AddModelError("Name", "Kindly Provide Country");
            }
            if (ModelState.IsValid)
            {
                int s = 100;
                string user_session = this.HttpContext.Session["username_session"].ToString();
                // string ratingid = model.rating_id.ToString();
                string name = model.name.ToString();
                string sname = model.sname.ToString();

                string constr = ConfigurationManager.ConnectionStrings["DAP_NEW"].ConnectionString;
                using (SqlConnection con = new SqlConnection(constr))
                {
                    con.Open();
                    // MessageBox.Show ("Connection Open ! ");
                    // cnn.Close();
                    SqlCommand cmd = new SqlCommand("SP_INSERT_STATE", con);    // to be corrected 
                    // SqlCommand cmd1 = new SqlCommand("select top 1 * from Customer_IDs where ID_NO ='" + txtCnicNo + "'", oCnn);   
                    cmd.CommandType = CommandType.StoredProcedure;
                    //cmd.Parameters.AddWithValue("@Rating_Id", model.rating_id);
                    cmd.Parameters.AddWithValue("@State_Name", model.name);
                    cmd.Parameters.AddWithValue("@State_ShortName", model.sname);
                    cmd.Parameters.AddWithValue("@STATE_STATUS", model.STATE_STATUS);
                    cmd.Parameters.AddWithValue("@country_id", Int32.Parse(model.country_id));
                    cmd.Parameters.AddWithValue("@sUser", user_session);
                    s = cmd.ExecuteNonQuery();
                    con.Close();
                }

                return RedirectToAction("State", "Home");
            }

            else
            {
                DataTable table = new DataTable();
                string constr = ConfigurationManager.ConnectionStrings["DAP_NEW"].ConnectionString;
                using (SqlConnection con = new SqlConnection(constr))
                {
                    //string query="select RATING_ID,RATING_NAME,RATING_SHORTNAME,RATING_STATUS,RATING_ISDEFAULT from config_rating ";
                    using (SqlCommand cmd = new SqlCommand("SP_GET_REGION_STATE", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        con.Open();
                        SqlDataAdapter ds = new SqlDataAdapter(cmd);
                        ds.Fill(table);
                        con.Close();
                    }
                }

                List<State> s_list = new List<State>();

                for (int i = 0; i < table.Rows.Count; i++)
                {
                    s_list.Add(new State()
                    {
                        id = table.Rows[i]["STATE_ID"].ToString(),
                        name = table.Rows[i]["STATE_NAME"].ToString(),
                        sname = table.Rows[i]["STATE_SHORTNAME"].ToString(),
                        STATE_STATUS = table.Rows[i]["STATE_STATUS"].ToString(), 
                        country_id = table.Rows[i]["COUNTRY_ID"].ToString(),
                        country_name = table.Rows[i]["COUNTRY_OFFICIALNAME"].ToString(),
                    });
                }

                DataTable table1 = new DataTable();
                using (SqlConnection con = new SqlConnection(constr))
                {
                    //string query="select RATING_ID,RATING_NAME,RATING_SHORTNAME,RATING_STATUS,RATING_ISDEFAULT from config_rating ";
                    using (SqlCommand cmd = new SqlCommand("SP_GET_REGION_COUNTRY", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        con.Open();
                        SqlDataAdapter ds = new SqlDataAdapter(cmd);
                        ds.Fill(table1);
                        con.Close();
                    }
                }

                List<State> c_list = new List<State>();

                for (int i = 0; i < table1.Rows.Count; i++)
                {
                    c_list.Add(new State()
                    {

                        country_id = table1.Rows[i]["COUNTRY_ID"].ToString(),
                        country_name = table1.Rows[i]["COUNTRY_OFFICIALNAME"].ToString(),

                    });
                }
                State user1 = new State();
                user1.state_list = s_list;
                user1.country_list = c_list;
                return View(user1);                
               // return RedirectToAction("State", "Home");
            }
        
        }
        public JsonResult edit_state(State model)
        {
            int s = 100;
            string user_session = this.HttpContext.Session["username_session"].ToString();
            string ratingid = model.id.ToString();
            string ratingname = model.name.ToString();
            string ratingshortname = model.sname.ToString();


            string constr = ConfigurationManager.ConnectionStrings["DAP_NEW"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                con.Open();
                // MessageBox.Show ("Connection Open ! ");
                // cnn.Close();
                SqlCommand cmd = new SqlCommand("SP_UPDATE_STATE", con);    // to be corrected 
                // SqlCommand cmd1 = new SqlCommand("select top 1 * from Customer_IDs where ID_NO ='" + txtCnicNo + "'", oCnn);   
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", model.id);
                cmd.Parameters.AddWithValue("@Name", model.name);
                cmd.Parameters.AddWithValue("@ShortName", model.sname);
                cmd.Parameters.AddWithValue("@STATE_STATUS", model.STATE_STATUS);
                cmd.Parameters.AddWithValue("@country_id", model.country_id);
                //cmd.Parameters.AddWithValue("@Rating_Status", model.rating_status);
                //cmd.Parameters.AddWithValue("@Rating_IsDefault", ratingisdefault);
                cmd.Parameters.AddWithValue("@sMUser", user_session);
                s = cmd.ExecuteNonQuery();
                con.Close();
            }
            string message1 = "Successful";
            string message2 = "unsuccessful";
            if (s == 0)
            {
                //TempData["Success"] = "Un Successfull";
                //return RedirectToAction("Rating", "Home");                
                return Json(message2, JsonRequestBehavior.AllowGet);
            }
            else
            {
                //TempData["Success"] = "Edit Successfully!";
                //return RedirectToAction("Rating", "Home");
                return Json(message1, JsonRequestBehavior.AllowGet);
            }
            //return View();
        }
        public ActionResult City()
        {

             if (this.HttpContext.Session["username_session"] != null)
     {

            DataTable table = new DataTable();
            string constr = ConfigurationManager.ConnectionStrings["DAP_NEW"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                //string query="select RATING_ID,RATING_NAME,RATING_SHORTNAME,RATING_STATUS,RATING_ISDEFAULT from config_rating ";
                using (SqlCommand cmd = new SqlCommand("SP_GET_REGION_COUNTRY", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    con.Open();
                    SqlDataAdapter ds = new SqlDataAdapter(cmd);
                    ds.Fill(table);
                    con.Close();
                }
            }

            List<City> c_list = new List<City>();
            for (int i = 0; i < table.Rows.Count; i++)
            {
                c_list.Add(new City()
                {
                    country_id = table.Rows[i]["COUNTRY_ID"].ToString(),
                    country_name = table.Rows[i]["COUNTRY_OFFICIALNAME"].ToString(),
                   // sname = table.Rows[i]["COUNTRY_SHORTNAME"].ToString(),
                });
            }
          
        /////////////////////////////////////////////////////////////////

            DataTable table1 = new DataTable();
            using (SqlConnection con = new SqlConnection(constr))
            {
                //string query="select RATING_ID,RATING_NAME,RATING_SHORTNAME,RATING_STATUS,RATING_ISDEFAULT from config_rating ";
                using (SqlCommand cmd = new SqlCommand("SP_GET_REGION_STATE", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    con.Open();
                    SqlDataAdapter ds = new SqlDataAdapter(cmd);
                    ds.Fill(table1);
                    con.Close();
                }
            }

            List<City> s_list = new List<City>();

            for (int i = 0; i < table1.Rows.Count; i++)
            {
                s_list.Add(new City()
                {
                    state_id = table1.Rows[i]["STATE_ID"].ToString(),
                    state_name = table1.Rows[i]["STATE_NAME"].ToString(),
                    // sname = table.Rows[i]["COUNTRY_SHORTNAME"].ToString(),
                });
            }

            ///////////////////////////////////////////////////////////////////////////////

            DataTable table2 = new DataTable();
            using (SqlConnection con = new SqlConnection(constr))
            {
                //string query="select RATING_ID,RATING_NAME,RATING_SHORTNAME,RATING_STATUS,RATING_ISDEFAULT from config_rating ";
                using (SqlCommand cmd = new SqlCommand("SP_GET_REGION_CITY", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    con.Open();
                    SqlDataAdapter ds = new SqlDataAdapter(cmd);
                    ds.Fill(table2);
                    con.Close();
                }
            }

            List<City> city_list = new List<City>();
            for (int i = 0; i < table2.Rows.Count; i++)
            {
                city_list.Add(new City()
                {
                    id = table2.Rows[i]["CITY_ID"].ToString(),
                    name = table2.Rows[i]["CITY_NAME"].ToString(),
                    sname = table2.Rows[i]["CITY_SHORTNAME"].ToString(),
                    CITY_STATUS = table2.Rows[i]["CITY_STATUS"].ToString(),                    
                    state_id = table2.Rows[i]["STATE_ID"].ToString(),
                    state_name = table2.Rows[i]["STATE_NAME"].ToString(),
                    country_id = table2.Rows[i]["COUNTRY_ID"].ToString(),
                    country_name = table2.Rows[i]["COUNTRY_OFFICIALNAME"].ToString(),
                    // sname = table.Rows[i]["COUNTRY_SHORTNAME"].ToString(),
                });
            }
            City user1 = new City();
            user1.country_list = c_list;
            user1.state_list = s_list;
            user1.city_list = city_list;
            return View(user1);
    }
             else
             {
                 return RedirectToAction("login", "Home");
             }

             }
        [HttpPost]
        public ActionResult City(City model)
        {

            if (string.IsNullOrEmpty(model.name))
            {
             //   ModelState.AddModelError("Name", "Kindly Provide Name");
            }
            if (string.IsNullOrEmpty(model.sname))
            {
              //  ModelState.AddModelError("Name", "Kindly Provide Name");
            }
            if (string.IsNullOrEmpty(model.CITY_STATUS))
            {
                //  ModelState.AddModelError("SName", "Kindly Provide Name");
            }
            
            
            if (string.IsNullOrEmpty(model.country_id))
            {
                ModelState.AddModelError("Name", "Kindly Provide Country");
            }
            if (string.IsNullOrEmpty(model.state_id))
            {
                ModelState.AddModelError("Name", "Kindly Provide Country");
            }

            if (ModelState.IsValid)
            {
                int s = 100;
                string user_session = this.HttpContext.Session["username_session"].ToString();
                // string ratingid = model.rating_id.ToString();
                string name = model.name.ToString();
                string sname = model.sname.ToString();

                string constr = ConfigurationManager.ConnectionStrings["DAP_NEW"].ConnectionString;
                using (SqlConnection con = new SqlConnection(constr))
                {
                    con.Open();
                    // MessageBox.Show ("Connection Open ! ");
                    // cnn.Close();
                    SqlCommand cmd = new SqlCommand("SP_INSERT_CITY", con);    // to be corrected 
                    // SqlCommand cmd1 = new SqlCommand("select top 1 * from Customer_IDs where ID_NO ='" + txtCnicNo + "'", oCnn);   
                    cmd.CommandType = CommandType.StoredProcedure;
                    //cmd.Parameters.AddWithValue("@Rating_Id", model.rating_id);
                    cmd.Parameters.AddWithValue("@City_Name",model.name);
                    cmd.Parameters.AddWithValue("@City_ShortName",model.sname);
                    cmd.Parameters.AddWithValue("@CITY_STATUS", model.CITY_STATUS);
                    cmd.Parameters.AddWithValue("@country_id",Int32.Parse(model.country_id));
                    cmd.Parameters.AddWithValue("@state_id",Int32.Parse(model.state_id));
                    cmd.Parameters.AddWithValue("@sUser",user_session);
                    s = cmd.ExecuteNonQuery();
                    con.Close();
                }

                return RedirectToAction("City", "Home");
            }

            else
            {
                DataTable table = new DataTable();
                string constr = ConfigurationManager.ConnectionStrings["DAP_NEW"].ConnectionString;
                using (SqlConnection con = new SqlConnection(constr))
                {
                    //string query="select RATING_ID,RATING_NAME,RATING_SHORTNAME,RATING_STATUS,RATING_ISDEFAULT from config_rating ";
                    using (SqlCommand cmd = new SqlCommand("SP_GET_REGION_COUNTRY", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        con.Open();
                        SqlDataAdapter ds = new SqlDataAdapter(cmd);
                        ds.Fill(table);
                        con.Close();
                    }
                }

                List<City> c_list = new List<City>();

                for (int i = 0; i < table.Rows.Count; i++)
                {
                    c_list.Add(new City()
                    {
                        country_id = table.Rows[i]["COUNTRY_ID"].ToString(),
                        country_name = table.Rows[i]["COUNTRY_OFFICIALNAME"].ToString(),
                        // sname = table.Rows[i]["COUNTRY_SHORTNAME"].ToString(),

                    });
                }

                /////////////////////////////////////////////////////////////////

                DataTable table1 = new DataTable();
                using (SqlConnection con = new SqlConnection(constr))
                {
                    //string query="select RATING_ID,RATING_NAME,RATING_SHORTNAME,RATING_STATUS,RATING_ISDEFAULT from config_rating ";
                    using (SqlCommand cmd = new SqlCommand("SP_GET_REGION_STATE", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        con.Open();
                        SqlDataAdapter ds = new SqlDataAdapter(cmd);
                        ds.Fill(table1);
                        con.Close();
                    }
                }

                List<City> s_list = new List<City>();

                for (int i = 0; i < table1.Rows.Count; i++)
                {
                    s_list.Add(new City()
                    {
                        state_id = table1.Rows[i]["STATE_ID"].ToString(),
                        state_name = table1.Rows[i]["STATE_NAME"].ToString(),
                        // sname = table.Rows[i]["COUNTRY_SHORTNAME"].ToString(),
                    });
                }

                ///////////////////////////////////////////////////////////////////////////////

                DataTable table2 = new DataTable();
                using (SqlConnection con = new SqlConnection(constr))
                {
                    //string query="select RATING_ID,RATING_NAME,RATING_SHORTNAME,RATING_STATUS,RATING_ISDEFAULT from config_rating ";
                    using (SqlCommand cmd = new SqlCommand("SP_GET_REGION_CITY", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        con.Open();
                        SqlDataAdapter ds = new SqlDataAdapter(cmd);
                        ds.Fill(table2);
                        con.Close();
                    }
                }

                List<City> city_list = new List<City>();

                for (int i = 0; i < table2.Rows.Count; i++)
                {
                    city_list.Add(new City()
                    {
                        id = table2.Rows[i]["CITY_ID"].ToString(),
                        name = table2.Rows[i]["CITY_NAME"].ToString(),
                        sname = table2.Rows[i]["CITY_SHORTNAME"].ToString(),
                        CITY_STATUS = table2.Rows[i]["CITY_STATUS"].ToString(),
                        
                        state_id = table2.Rows[i]["STATE_ID"].ToString(),
                        state_name = table2.Rows[i]["STATE_NAME"].ToString(),
                        country_id = table2.Rows[i]["COUNTRY_ID"].ToString(),
                        country_name = table2.Rows[i]["COUNTRY_OFFICIALNAME"].ToString(),
                        // sname = table.Rows[i]["COUNTRY_SHORTNAME"].ToString(),
                    });
                }

                City user1 = new City();
                user1.country_list = c_list;
                user1.state_list = s_list;
                user1.city_list = city_list;
                return View(user1);

            }

        }
        public JsonResult edit_city(City model)
        {
            int s = 100;
            string user_session = this.HttpContext.Session["username_session"].ToString();
            string ratingid = model.id.ToString();
            string ratingname = model.name.ToString();
            string ratingshortname = model.sname.ToString();

            string constr = ConfigurationManager.ConnectionStrings["DAP_NEW"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                con.Open();
                // MessageBox.Show ("Connection Open ! ");
                // cnn.Close();
                SqlCommand cmd = new SqlCommand("SP_UPDATE_CITY", con);    // to be corrected 
                // SqlCommand cmd1 = new SqlCommand("select top 1 * from Customer_IDs where ID_NO ='" + txtCnicNo + "'", oCnn);   
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", model.id);
                cmd.Parameters.AddWithValue("@Name", model.name);
                cmd.Parameters.AddWithValue("@ShortName", model.sname);
                cmd.Parameters.AddWithValue("@CITY_STATUS", model.CITY_STATUS);
                cmd.Parameters.AddWithValue("@country_id", model.country_id);
                cmd.Parameters.AddWithValue("@state_id", model.state_id);
                //cmd.Parameters.AddWithValue("@Rating_Status", model.rating_status);
                //cmd.Parameters.AddWithValue("@Rating_IsDefault", ratingisdefault);
                cmd.Parameters.AddWithValue("@sMUser", user_session);
                s = cmd.ExecuteNonQuery();
                con.Close();
            }
            string message1 = "Successful";
            string message2 = "unsuccessful";
            if (s == 0)
            {
                //TempData["Success"] = "Un Successfull";
                //return RedirectToAction("Rating", "Home");                
                return Json(message2, JsonRequestBehavior.AllowGet);
            }
            else
            {
                //TempData["Success"] = "Edit Successfully!";
                //return RedirectToAction("Rating", "Home");
                return Json(message1, JsonRequestBehavior.AllowGet);
            }
            //return View();
        }
        //[HttpPost]
        //public ActionResult bulkposting_process(Trandetail model)
        //{          
        //   return View();
        //}

       [HttpPost]
        public ActionResult bulkposting_process(trandetail_fund model)
        {
            int s = 0;
            
            string constr = ConfigurationManager.ConnectionStrings["DAP_NEW"].ConnectionString;
            if (model.Val_Date > DateTime.Now.Date)
            {
                ModelState.AddModelError("Val_Date", "kindly enter value date less then post date");
            }

            DataTable table = new DataTable();
            using (SqlConnection con = new SqlConnection(constr))
            {
                //string query = "SELECT * FROM CONFIG_ACCOUNTPURPOSE ";
                using (SqlCommand cmd = new SqlCommand("SP_GET_TRANDETAIL_ALL", con))   //replace the stored procedure name with string name defined above
                {
                    var dateAndTime = DateTime.Now;
                    var valdate = Convert.ToDateTime(model.Val_Date).ToString("yyyy-MM-dd");                   //"2018-06-20";
                    var postdate = Convert.ToDateTime(DateTime.Now.Date).ToString("yyyy-MM-dd");                //"2018-06-21" ;
                    //int s = 1;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@valuDate", valdate);
                    cmd.Parameters.AddWithValue("@postDate", postdate);
                    SqlDataAdapter ds = new SqlDataAdapter(cmd);
                    ds.Fill(table);
                }
            }

            if (table.Rows.Count==0)
            {
                ModelState.AddModelError("Val_Date", "No balance found for given value date");
            }
            if (ModelState.IsValid)
            {
                string user_session = this.HttpContext.Session["username_session"].ToString();
                string fundid = model.FUND_ID;
                string vdate = Convert.ToDateTime(model.Val_Date).ToString("yyyy-MM-dd");
                string pdate = Convert.ToDateTime(DateTime.Now.Date).ToString("yyyy-MM-dd");
                //this.HttpContext.Session["VALUE_DATE"] = vdate;
                //this.HttpContext.Session["fundid_session"] = fundid;
                //this.HttpContext.Session["POST_DATE"] = pdate;
                //this.HttpContext.Session["option_session"] = model.search_option;
                //this.HttpContext.Session["bankid_session"] = model.BANK_ID;
                //this.HttpContext.Session["fund_bank_acc_id"] = model.FUND_BANK_ACC_ID;
                ////////////////////////////////////////////////////////////////////////////////////

                using (SqlConnection con = new SqlConnection(constr))
                {
                    con.Open();
                    // MessageBox.Show ("Connection Open ! ");
                    // cnn.Close();
                    SqlCommand cmd = new SqlCommand("SP_Update_Post_TRANDETAIL_ALL", con);    // to be corrected 
                    // SqlCommand cmd1 = new SqlCommand("select top 1 * from Customer_IDs where ID_NO ='" + txtCnicNo + "'", oCnn);   
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@valuDate", vdate);
                    cmd.Parameters.AddWithValue("@user", user_session);
                    s = cmd.ExecuteNonQuery();
                    con.Close();
                }
                string message1 = "Successful";
                string message2 = "unsuccessful";
                if (s == 0)
                {
                    return Json(message2, JsonRequestBehavior.AllowGet);
                }
                else
                {

                   ViewBag.Message = "Transfer Successful";


                   this.HttpContext.Session["message successfull"] = "Posted Successfully";


                  //  return trandetail_fund_setup();


                 //  ViewBag.Message = "";


                    return RedirectToAction("trandetail_fund_setup");


                  // RedirectToAction("trandetail_fund_setup");
                  // return Json(message1, JsonRequestBehavior.AllowGet);

                  // return Json( new { result = "Redirect", url = Url.Action("trandetail_fund_setup", "HomeController") });

                    
                
                }

            }
            else
            {
                return Json("no balances found or already posted for given date", JsonRequestBehavior.AllowGet);
            }

           // return View();
        }
        
        
        public ActionResult Balance_Transfer()
        {
            if (this.HttpContext.Session["username_session"] == null)
            {
                return RedirectToAction("login", "Home");
            }
            
            if (this.HttpContext.Session["message_balancetransfer_successful"] == null)
            {
                ViewBag.Message = null;
            }
            else { ViewBag.Message = this.HttpContext.Session["message_balancetransfer_successful"].ToString(); }

            this.HttpContext.Session["message_balancetransfer_successful"] = null;
                        
            trandetail_fund user1 = new trandetail_fund();         
            user1.post_date_string = Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd");
            return View(user1);
        }

        [HttpPost]
        public ActionResult Balance_Transfer(trandetail_fund model)
        {
            {
                // string OPT = Request.Form["radio_fund"].ToString();

                string constr = ConfigurationManager.ConnectionStrings["DAP_NEW"].ConnectionString;


                if (model.Val_Date ==null || model.Transfer_Date==null)
                {
                    ModelState.AddModelError("Val_Date", "kindly provide correct dates");
                }



                if (model.Val_Date > DateTime.Now.Date)
                {
                    ModelState.AddModelError("Val_Date", "kindly enter value date less than post date");
                }
                if (model.Val_Date>=model.Transfer_Date)
                {
                    ModelState.AddModelError("Val_Date", "kindly provide valdate less than transfer date");
                }

                    string check_date = Convert.ToDateTime(model.Transfer_Date).ToString("yyyy-MM-dd");
                    DataTable table_ = new DataTable();

        using (SqlConnection con = new SqlConnection(constr))
         {
                    //string query = "SELECT * FROM CONFIG_ACCOUNTPURPOSE ";
   using (SqlCommand cmd = new SqlCommand("select * from TXN_TRANMASTER where TXNMSTR_VALDATE='" + check_date+"'", con))   //replace the stored procedure name with string name defined above
            {
                         //"2018-06-21" ;
                        //int s = 1;
                        cmd.CommandType = CommandType.Text;
                        SqlDataAdapter ds = new SqlDataAdapter(cmd);
                        ds.Fill(table_);
                    }
                }


        if (table_.Rows.Count>0)
        {
            ModelState.AddModelError("Val_Date", "balance for given transfer date already placed");
        }
                
                if (ModelState.IsValid)
                {

                    string fundid = model.FUND_ID;
                    string vdate = Convert.ToDateTime(model.Val_Date).ToString("yyyy-MM-dd");
                    string pdate = Convert.ToDateTime(DateTime.Now.Date).ToString("yyyy-MM-dd");
                    string tdate = Convert.ToDateTime(model.Transfer_Date).ToString("yyyy-MM-dd");
                    this.HttpContext.Session["VALUE_DATE"] = vdate;
                    this.HttpContext.Session["transferdate"] = tdate;
                    this.HttpContext.Session["POST_DATE"] = pdate;
                  

                    string user_session = this.HttpContext.Session["username_session"].ToString();


                    //  string constr = ConfigurationManager.ConnectionStrings["DAP_NEW"].ConnectionString;
                    DataTable table = new DataTable();
                    using (SqlConnection con = new SqlConnection(constr))
                    {
                        //string query = "SELECT * FROM CONFIG_ACCOUNTPURPOSE ";
                        using (SqlCommand cmd = new SqlCommand("SP_GET_TRANDETAIL_Balance_transfer", con))   //replace the stored procedure name with string name defined above
                        {
                            var dateAndTime = DateTime.Now;
                            var valdate = this.HttpContext.Session["VALUE_DATE"].ToString();                   //"2018-06-20";
                            var postdate = this.HttpContext.Session["POST_DATE"].ToString();                //"2018-06-21" ;
                            //int s = 1;
                            cmd.CommandType = CommandType.StoredProcedure;

                          
                         //  cmd.Parameters.AddWithValue("@bankid", bkid);                          
                           cmd.Parameters.AddWithValue("@valuDate", valdate);
                           cmd.Parameters.AddWithValue("@postDate", postdate);


                            SqlDataAdapter ds = new SqlDataAdapter(cmd);
                            ds.Fill(table);
                        }
                    }

                    List<trandetail_fund> trandetail_setup_list = new List<trandetail_fund>();
                    for (int i = 0; i < table.Rows.Count; i++)
                    {
                       
                         string  tf_txnd_id = table.Rows[i]["txnd_id"].ToString();
                            string tf_txnmstr_id = table.Rows[i]["txnmstr_id"].ToString();
                           string tf_fbacc_id = table.Rows[i]["fbacc_id"].ToString();
                            //FUND_ID = table.Rows[i]["FUND_ID"].ToString(),
                            //FUND_NAME = table.Rows[i]["FUND_NAME"].ToString(),
                          string  tf_fund_id = table.Rows[i]["fund_id"].ToString();
                         string   tf_fund_name = table.Rows[i]["fund_name"].ToString();
                          string  tf_bank_name = table.Rows[i]["bank_name"].ToString();
                         string   tf_bbr_name = table.Rows[i]["bbr_name"].ToString();
                          string  tf_fbacc_number = table.Rows[i]["fbacc_number"].ToString();
                            //txnd_type = table.Rows[i]["txnd_type"].ToString(),
                         string   tf_txnd_amount = table.Rows[i]["txnd_amount"].ToString();
                          string  tf_ptrate_percent = table.Rows[i]["ptrate_percent"].ToString();
                          string  tf_profit = table.Rows[i]["profit"].ToString();

                          string  tf_txnd_l_amount = table.Rows[i]["txnd_l_amount"].ToString();
      
                       
                        
                       // string  tf_last_valueDate = Convert.ToDateTime(table.Rows[i]["last_ValueDate"]).ToString("yyyy-MM-dd");
                          string  tf_value_date = Convert.ToDateTime(table.Rows[i]["ValueDate"]).ToString("yyyy-MM-dd");
                          string  tf_postDate = Convert.ToDateTime(table.Rows[i]["PostDate"]).ToString("yyyy-MM-dd");

                            // value_date = table.Rows[i]["ValueDate"].ToString(),
                          string  tf_txnd_status = table.Rows[i]["txnd_status"].ToString();
                          string tf_txnd_postedon = table.Rows[i]["txnd_postedon"].ToString();


                      

                using (SqlConnection con = new SqlConnection(constr))
                 {

                     con.Open();
                     // MessageBox.Show ("Connection Open ! ");
                     // cnn.Close();
                     SqlCommand cmd = new SqlCommand("SP_INSERT_Tran_Detail_MSTR_BalanceTransfer", con);    // to be corrected 
                     // SqlCommand cmd1 = new SqlCommand("select top 1 * from Customer_IDs where ID_NO ='" + txtCnicNo + "'", oCnn);   
                     cmd.CommandType = CommandType.StoredProcedure;
                     cmd.Parameters.AddWithValue("@TXNMSTR_VALDATE", Convert.ToDateTime(this.HttpContext.Session["transferdate"]).ToString("yyyy-MM-dd"));

                     cmd.Parameters.AddWithValue("@TXNMSTR_POSTDATE", DateTime.Now.Date);
                     cmd.Parameters.AddWithValue("@FBACC_ID", Int32.Parse(table.Rows[i]["fbacc_id"].ToString()));
                    // cmd.Parameters.AddWithValue("@PTRATE_PERCENT", table.Rows[i]["ptrate_percent"].ToString());
                     cmd.Parameters.AddWithValue("@TXND_AMOUNT", table.Rows[i]["txnd_amount"].ToString());
                    // cmd.Parameters.AddWithValue("@TXND_ProfitAMOUNT", table.Rows[i]["profit"].ToString());
                     cmd.Parameters.AddWithValue("@sUser", user_session);
                    int  s_ = cmd.ExecuteNonQuery();
                     con.Close();

                 }
                    
                    
                    }
                    //Trandetail user1 = new Trandetail();
                    //user1.trandetail_list = trandetail_setup_list;

                    int s2 = 200;
                    string constr2 = ConfigurationManager.ConnectionStrings["DAP_NEW"].ConnectionString;

                    ViewBag.Message = string.Format("Balance Transfer Successfull");
                    this.HttpContext.Session["message_balancetransfer_successful"] = "Balance Transfer Successfull";
             
                    trandetail_fund user1 = new trandetail_fund();                  
                    user1.post_date_string = Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd");
                    return View(user1);

                    ////////////////////////////////////////////////

                    //     return RedirectToAction("trandetail_setup", "Home");
                }
                else
                {
                    

                    List<trandetail_fund> trandetail_setup_list12 = new List<trandetail_fund>();

                    trandetail_fund user1 = new trandetail_fund();
                    //user1.FUND_OPTION_LIST = fund_list19;
                    //user1.BANK_OPTION_LIST = bank_list19;
                    //user1.trandetail_fund_list = trandetail_setup_list12;
                    user1.post_date_string = Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd");
                    return View(user1);
                }
                // }      
            }
        }


        public ActionResult Process()
        {
            if (this.HttpContext.Session["username_session"] == null)
            {
                return RedirectToAction("login", "Home");

            }
            
            
            
            
            
            if (this.HttpContext.Session["message_Process_successful"] == null)
            {
                ViewBag.Message = null;
            }
            else { ViewBag.Message = this.HttpContext.Session["message_Process_successful"].ToString(); }

            this.HttpContext.Session["message_Process_successful"] = null;
            
            
            Process user1 = new Process();        
            return View(user1);
        }

        [HttpPost]
        public ActionResult Process(Process model)
        {

            string constr = ConfigurationManager.ConnectionStrings["DAP_NEW"].ConnectionString;

            if (model.Purpose == "")
            {
                ModelState.AddModelError("Purpose", "kindly provide process type");
            }

            if (model.ProcessDate == null)
            {
                ModelState.AddModelError("Process Date", "kindly provide process date");
            }

            string check_date = Convert.ToDateTime(model.ProcessDate).ToString("yyyy-MM-dd");
            string check_purpose = model.Purpose.ToString();


            DataTable table_ = new DataTable();
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand("select * from process where processtype='Daily' and date='" + check_date + "' and purpose='" + check_purpose + "'", con))   //replace the stored procedure name with string name defined above
                {
                    //"2018-06-21" ;
                    //int s = 1;
                    cmd.CommandType = CommandType.Text;
                    SqlDataAdapter ds = new SqlDataAdapter(cmd);
                    ds.Fill(table_);
                }
            }
            if (table_.Rows.Count > 0)
            {
                ModelState.AddModelError("Process Date", "Process already executed for the given date");
            }

            DataTable table_fbaccnt = new DataTable();
            using (SqlConnection con = new SqlConnection(constr))
            {
                string QUERY_FBACCNT
               = "select * from STP_FUNDBANKACCOUNT where PFRQ_ID ='3' and FBACC_ISCLOSED = '0' and FBACC_OPENINGDATE <='" + check_date + "'";
                using (SqlCommand cmd = new SqlCommand(QUERY_FBACCNT, con))   //replace the stored procedure name with string name defined above
                {
                    //"2018-06-21" ;
                    //int s = 1;
                    cmd.CommandType = CommandType.Text;
                    SqlDataAdapter ds = new SqlDataAdapter(cmd);
                    ds.Fill(table_fbaccnt);
                }
            }

            DataTable table_trandetail = new DataTable();
            using (SqlConnection con = new SqlConnection(constr))
            {
                string QUERY_TranDetail
                = "select * from TXN_TRANMASTER tm inner join TXN_TRANDETAIL td on tm.TXNMSTR_ID=td.TXNMSTR_ID where tm.txnmstr_status='post' and td.TXND_STATUS='post' and tm.txnmstr_valdate='" + check_date + "'";
                using (SqlCommand cmd = new SqlCommand(QUERY_TranDetail, con))   //replace the stored procedure name with string name defined above
                {
                    cmd.CommandType = CommandType.Text;
                    SqlDataAdapter ds = new SqlDataAdapter(cmd);
                    ds.Fill(table_trandetail);
                }

                con.Close();

            }

            if (table_fbaccnt.Rows.Count != table_trandetail.Rows.Count)
            {
                ModelState.AddModelError("Process Date", "All Account Balances are not entered for this period");
            }
            if (ModelState.IsValid)
            {
                using (SqlConnection con = new SqlConnection(constr))
                {
                    con.Open();
                    string QUERY_delete_rpt_balance
                    = "delete from Rpt_BankBalance";
                    using (SqlCommand cmd = new SqlCommand(QUERY_delete_rpt_balance, con))   //replace the stored procedure name with string name defined above
                    {
                        cmd.CommandType = CommandType.Text;
                        int s = cmd.ExecuteNonQuery();
                    }
                    con.Close();

                }    /////

                string account_pur = "";
                if (model.Purpose.ToString() != "NORMAL")
                {
                    account_pur = "2";

                }
                else 
                {
                    account_pur = "1";
                }
                DataTable table_bal_bank_branch = new DataTable();
                using (SqlConnection con = new SqlConnection(constr))
                {
                    string sum = " insert into Rpt_BankBalance ";
                    sum = sum + "select bank_name as bank,bbr_name as branch,sum(TXND_AMOUNT) as balance,null from STP_FUNDBANKACCOUNT fb";
                    sum = sum + " inner join  TXN_TRANMASTER tm on tm.FBACC_ID= fb.FBACC_ID";
                    sum = sum + " inner join  TXN_TRANDETAIL td on tm.TXNMSTR_ID=td.TXNMSTR_ID";
                    sum = sum + " inner join  stp_bank bnk on bnk.bank_id=fb.bank_id";
                    sum = sum + " inner join  stp_bankbranch br on br.bbr_id=fb.bbr_id";
                    sum = sum + " where  tm.TXNMSTR_STATUS='POST'";
                    sum = sum + "  and td.TXND_STATUS='POST' AND fb.FBACC_ISPOOLED='true' and fb.accpur_id='" + account_pur + "'";
                    sum = sum + " and tm.TXNMSTR_VALDATE='" + check_date + "'";
                    sum = sum + " group by bank_name,bbr_name";
                    using (SqlCommand cmd = new SqlCommand(sum, con))   //replace the stored procedure name with string name defined above
                    {
                        con.Open();
                        cmd.CommandType = CommandType.Text;
                        int s = cmd.ExecuteNonQuery();
                        //SqlDataAdapter ds = new SqlDataAdapter(cmd);
                        //ds.Fill(table_bal_bank_branch);
                    }
                    con.Close();
                }  ////

                using (SqlConnection con = new SqlConnection(constr))
                {
                    con.Open();
                    string QUERY_delete_rpt_SLAB
                    = "delete from Rpt_SLAB";
                    using (SqlCommand cmd = new SqlCommand(QUERY_delete_rpt_SLAB, con))   //replace the stored procedure name with string name defined above
                    {
                        cmd.CommandType = CommandType.Text;
                        int s = cmd.ExecuteNonQuery();
                    }
                    con.Close();

                }  ////
                DataTable table1 = new DataTable();
                using (SqlConnection con = new SqlConnection(constr))
                {
                    string QUERY_TABLE1
                    = "select * from Rpt_BankBalance";
                    using (SqlCommand cmd = new SqlCommand(QUERY_TABLE1, con))   //replace the stored procedure name with string name defined above
                    {
                        cmd.CommandType = CommandType.Text;
                        SqlDataAdapter ds = new SqlDataAdapter(cmd);
                        ds.Fill(table1);
                    }
                    con.Close();
                }////

                for (int i = 0; i < table1.Rows.Count; i++)
                {
                    string BAL = "SELECT * FROM STP_BALAMOUNTSLABS S INNER JOIN ( SELECT * FROM rpt_bankbalance R INNER JOIN STP_BANK  BNK ON BNK.BANK_NAME=R.Bank ) RB ";
                    BAL = BAL + " ON S.BANK_ID=RB.BANK_ID  "; //--inner join stp_profitrate pr on pr.SLAB_ID=S.SLAB_ID 
                    BAL = BAL + " WHERE SLAB_AMOUNTFROM<='" + table1.Rows[i]["Balance"].ToString() + "' AND SLAB_AMOUNTTO>'" + table1.Rows[i]["Balance"].ToString() + "'";
                    BAL = BAL + " AND BANK_NAME='" + table1.Rows[i]["Bank"].ToString() + "'"; // AND ACCPUR_ID='"+account_pur+"'";



                    DataTable table2 = new DataTable();
                    using (SqlConnection con = new SqlConnection(constr))
                    {
                        using (SqlCommand cmd = new SqlCommand(BAL, con))   //replace the stored procedure name with string name defined above
                        {
                            cmd.CommandType = CommandType.Text;
                            SqlDataAdapter ds = new SqlDataAdapter(cmd);
                            ds.Fill(table2);
                        }
                    }
                    if (table2.Rows.Count == 0)
                    {
                        string BAL1 = "SELECT * FROM STP_BALAMOUNTSLABS S INNER JOIN ( SELECT * FROM rpt_bankbalance R INNER JOIN STP_BANK  BNK ON BNK.BANK_NAME=R.Bank ) RB ";
                        BAL1 = BAL1 + " ON S.BANK_ID=RB.BANK_ID";
                        BAL1 = BAL1 + " WHERE SLAB_AMOUNTFROM>='" + table1.Rows[i]["Balance"].ToString() + "' AND SLAB_AMOUNTTO<='" + table1.Rows[i]["Balance"].ToString() + "'";

                        //DataTable table2 = new DataTable();
                        using (SqlConnection con = new SqlConnection(constr))
                        {
                            using (SqlCommand cmd = new SqlCommand(BAL1, con))   //replace the stored procedure name with string name defined above
                            {
                                cmd.CommandType = CommandType.Text;
                                SqlDataAdapter ds = new SqlDataAdapter(cmd);
                                ds.Fill(table2);
                            }
                        }
                    }

                    for (int j = 0; j < table2.Rows.Count; j++)
                    {

                        using (SqlConnection con = new SqlConnection(constr))
                        {
                            con.Open();
                            // string QUERY_delete_rpt_SLAB
                            string query = "insert into rpt_slab values('" + table2.Rows[j]["Bank"].ToString() + "','" + table2.Rows[j]["Branch"].ToString() + "','" + table2.Rows[j]["SLAB_NAME"].ToString() + "',null)";
                            //Writer insert  query 

                            using (SqlCommand cmd = new SqlCommand(query, con))   //replace the stored procedure name with string name defined above
                            {
                                cmd.CommandType = CommandType.Text;
                                int s = cmd.ExecuteNonQuery();
                            }
                            con.Close();
                        }
                    }

                }/////////

                using (SqlConnection con = new SqlConnection(constr))
                {
                    con.Open();
                    string QUERY_delete_rpt_PROFIT
                    = "delete from Rpt_ProfitRate";
                    using (SqlCommand cmd = new SqlCommand(QUERY_delete_rpt_PROFIT, con))   //replace the stored procedure name with string name defined above
                    {
                        cmd.CommandType = CommandType.Text;
                        int s = cmd.ExecuteNonQuery();
                    }
                    con.Close();
                } ////

                DataTable table3 = new DataTable();
                using (SqlConnection con = new SqlConnection(constr))
                {
                    string QUERY_TABLE3
                    = " select distinct * from Rpt_Slab";
                    using (SqlCommand cmd = new SqlCommand(QUERY_TABLE3, con))   //replace the stored procedure name with string name defined above
                    {
                        cmd.CommandType = CommandType.Text;
                        SqlDataAdapter ds = new SqlDataAdapter(cmd);
                        ds.Fill(table3);
                    }
                    con.Close();
                } //

                if (table3.Rows.Count > 0) 
                {

                    for (int i = 0; i < table3.Rows.Count; i++)
                    {
   string pfcal = "insert into rpt_profitrate ";
  pfcal  = pfcal+ " SELECT bank,branch,ptrate_percent,null FROM STP_PROFITRATE PR INNER JOIN ( select * from Rpt_Slab R  INNER JOIN (SELECT BANK_ID BNKID,BANK_NAME BNKNAME FROM STP_BANK)   BNK ON BNK.BNKNAME=R.Bank  INNER JOIN (SELECT BBR_ID BRID, BBR_NAME BRNAME  FROM  STP_BANKBRANCH) BR ON BR.BRNAME=R.BRANCH  INNER JOIN (SELECT SLAB_ID SLBID,SLAB_NAME SLNAME FROM STP_BALAMOUNTSLABS) BLAMNT ON BLAMNT.SLNAME=R.Slab ) S ";
 pfcal =pfcal+ " ON  PR.SLAB_ID=S.SLBID INNER join (select ACCTYPE_ID typeid,ACCTYPE_NAME typename from config_accounttype) acctype on acctype.typeid=pr.acctype_id INNER join (select ACCPUR_ID purid,accpur_name purname from config_accountpurpose) acctpurpose on acctpurpose.purid=pr.ACCPUR_ID   ";
 pfcal = pfcal + " where  PTRATE_STATUS='ACTIVE' AND  PR.PTRATE_EFFECTIVEFROM <='" +check_date + "' AND PR.PTRATE_EFFECTIVETO>='" + check_date + "'";
 pfcal = pfcal + " AND BANK='" + table3.Rows[i]["Bank"].ToString() + "'" + "AND BRANCH ='" + table3.Rows[i]["Branch"].ToString() + "' and slab='" + table3.Rows[i]["Slab"].ToString() + "'";// and ACCPUR_ID='"+account_pur+"'";

                        //DataTable table4 = new DataTable();
                        using (SqlConnection con = new SqlConnection(constr))
                        {
                            con.Open();
                            using (SqlCommand cmd = new SqlCommand(pfcal, con))   //replace the stored procedure name with string name defined above
                            {
                                cmd.CommandType = CommandType.Text;
                                int s = cmd.ExecuteNonQuery();
                                //SqlDataAdapter ds = new SqlDataAdapter(cmd);
                                //ds.Fill(table4);
                            }
                            con.Close();
                        }

                    }
                
                }   /////
                string serial="";
                DataTable table4 = new DataTable();
                using (SqlConnection con = new SqlConnection(constr))
                {
string QUERY_TABLE4="";
QUERY_TABLE4 = QUERY_TABLE4 + " select  tm.txnmstr_valdate,fund_name,Bank_name,bbr_name,acctype_name,accpur_name,pbas_name";
QUERY_TABLE4 = QUERY_TABLE4 + " ,fbacc_ispooled,pfrq_name,fbacc_number,txnd_amount from STP_FUNDBANKACCOUNT fb ";
QUERY_TABLE4 = QUERY_TABLE4 + " inner join  TXN_TRANMASTER tm on tm.FBACC_ID= fb.FBACC_ID ";                    
QUERY_TABLE4 = QUERY_TABLE4 + " inner join  TXN_TRANDETAIL td on tm.TXNMSTR_ID=td.TXNMSTR_ID" ; 
QUERY_TABLE4 = QUERY_TABLE4 + " inner join  stp_bank bnk on bnk.bank_id=fb.bank_id";
QUERY_TABLE4 = QUERY_TABLE4 + " inner join  stp_bankbranch br on br.bbr_id=fb.bbr_id";                
QUERY_TABLE4 = QUERY_TABLE4 + " inner join  STP_FUND fund on fund.FUND_ID=fb.FUND_ID";
QUERY_TABLE4 = QUERY_TABLE4 + " inner join  CONFIG_ACCOUNTTYPE atype on atype.acctype_id=fb.ACCTYPE_ID";
QUERY_TABLE4 = QUERY_TABLE4 + " inner join  CONFIG_ACCOUNTPURPOSE apurpose on apurpose.accpur_id=fb.accpur_id";
QUERY_TABLE4 = QUERY_TABLE4 + " inner join  CONFIG_PROFITBASIS pbasis on pbasis.pbas_id=fb.pbas_id";
QUERY_TABLE4 = QUERY_TABLE4 + " inner join  CONFIG_PROFITFREQUENCY  pfreq on pfreq.pfrq_id=fb.pfrq_id";
QUERY_TABLE4 = QUERY_TABLE4 + " where  tm.TXNMSTR_STATUS='POST' and td.TXND_STATUS='POST' AND fb.FBACC_ISPOOLED='true'  and fb.PFRQ_ID='3'";
QUERY_TABLE4 = QUERY_TABLE4 + " and fb.accpur_id='"+account_pur+"' and tm.TXNMSTR_VALDATE='"+check_date+"'";
QUERY_TABLE4 = QUERY_TABLE4 + " group by tm.txnmstr_valdate,fund_name,Bank_name,bbr_name,acctype_name,accpur_name,pbas_name";
QUERY_TABLE4 = QUERY_TABLE4 + ",fbacc_ispooled,pfrq_name,fbacc_number,txnd_amount ";
                    
                    using (SqlCommand cmd = new SqlCommand(QUERY_TABLE4, con))   //replace the stored procedure name with string name defined above
                    {
                        cmd.CommandType = CommandType.Text;
                        SqlDataAdapter ds = new SqlDataAdapter(cmd);
                        ds.Fill(table4);
                    }
                    con.Close();
                }   ////////////

                for (int i = 0; i < table4.Rows.Count; i++)    /////////
                {
                    string query_table5 = "select distinct  * from Rpt_ProfitRate ";
                    //query_table5 =query_table5 + " ON S.BANK_ID=RB.BANK_ID";
                    query_table5 = query_table5 + " WHERE BANK='" + table4.Rows[i]["Bank_name"].ToString() + "'";
                    query_table5 = query_table5 + " AND BRANCH='" + table4.Rows[i]["bbr_name"].ToString() + "'";

                    DataTable table5 = new DataTable();
                    using (SqlConnection con = new SqlConnection(constr))
                    {
                        using (SqlCommand cmd = new SqlCommand(query_table5, con))   //replace the stored procedure name with string name defined above
                        {
                            cmd.CommandType = CommandType.Text;
                            SqlDataAdapter ds = new SqlDataAdapter(cmd);
                            ds.Fill(table5);
                        }
                    }

                    string query_table6 = "select CASE WHEN (Max(Serial) + 1) IS NULL THEN 1 ELSE (Max(Serial) + 1) END as NewId from Profit_Accrual";
                    DataTable table6 = new DataTable();
                    using (SqlConnection con = new SqlConnection(constr))
                    {
                        using (SqlCommand cmd = new SqlCommand(query_table6, con))   //replace the stored procedure name with string name defined above
                        {
                            cmd.CommandType = CommandType.Text;
                            SqlDataAdapter ds = new SqlDataAdapter(cmd);
                            ds.Fill(table6);
                        }
                    }

                    for (int j = 0; j < table5.Rows.Count; j++)
                    {
                        serial = table6.Rows[0]["NewId"].ToString();
                        string process_date = DateTime.Now.Date.ToString("yyyy-MM-dd");
                        float Accrual = float.Parse(table4.Rows[i]["txnd_amount"].ToString()) * float.Parse(table5.Rows[j]["ProfitRate"].ToString()) / 100;
                        Accrual = Accrual / 365;
                        string username = this.HttpContext.Session["username_session"].ToString();
                        string insert_profit_accural = "";

  insert_profit_accural=insert_profit_accural+"insert into Profit_Accrual values(@serial,@valudate,@processdate,@fundname,@bankname,@branchname,@type,@purpose,@profitbasis,@pool,@frequency,@accountno,@bankbalance,@profitrate,@accural,@username,@lastbalance)";

                        string ispooled = table4.Rows[i]["fbacc_ispooled"].ToString();

                        using (SqlConnection con = new SqlConnection(constr))
                        {
                            con.Open();
                            using (SqlCommand cmd = new SqlCommand(insert_profit_accural, con))   //replace the stored procedure name with string name defined above
                            {

                                string vlue_date = table4.Rows[i]["txnmstr_valdate"].ToString();
                                DateTime vlue_date_datetime = Convert.ToDateTime(vlue_date);
                                
                                cmd.CommandType = CommandType.Text;
                                cmd.Parameters.AddWithValue("@serial", serial);
                              //  cmd.Parameters.AddWithValue("@valudate", table4.Rows[i]["txnmstr_valdate"].ToString());

                                cmd.Parameters.AddWithValue("@valudate", vlue_date_datetime.ToString("yyyy-MM-dd"));
                                cmd.Parameters.AddWithValue("@processdate", process_date);
                                cmd.Parameters.AddWithValue("@fundname", table4.Rows[i]["fund_name"].ToString());
                                cmd.Parameters.AddWithValue("@bankname", table4.Rows[i]["Bank_name"].ToString());
                                cmd.Parameters.AddWithValue("@branchname", table4.Rows[i]["bbr_name"].ToString());
                                cmd.Parameters.AddWithValue("@type", table4.Rows[i]["acctype_name"].ToString());
                                cmd.Parameters.AddWithValue("@purpose", table4.Rows[i]["accpur_name"].ToString());
                                cmd.Parameters.AddWithValue("@profitbasis", table4.Rows[i]["pbas_name"].ToString());
                                cmd.Parameters.AddWithValue("@pool", ispooled);
                                cmd.Parameters.AddWithValue("@frequency", table4.Rows[i]["pfrq_name"].ToString());
                                cmd.Parameters.AddWithValue("@accountno", table4.Rows[i]["fbacc_number"].ToString());
                                cmd.Parameters.AddWithValue("@bankbalance", table4.Rows[i]["txnd_amount"].ToString());
                                cmd.Parameters.AddWithValue("@profitrate", table5.Rows[j]["ProfitRate"].ToString());
                                cmd.Parameters.AddWithValue("@accural", Accrual.ToString());
                                cmd.Parameters.AddWithValue("@username", username);
                               // cmd.Parameters.AddWithValue("@lastbalance", table4.Rows[i]["txnd_amount"].ToString());

                                cmd.Parameters.AddWithValue("@lastbalance", DBNull.Value);
                                
                                int s = cmd.ExecuteNonQuery();
                                //SqlDataAdapter ds = new SqlDataAdapter(cmd);
                                //ds.Fill(table4);
                            }
                            con.Close();
                        }


                    }




                    //using (SqlConnection con = new SqlConnection(constr))
                    //{

                    //    con.Open();
                    //    // MessageBox.Show ("Connection Open ! ");
                    //    // cnn.Close();
                    //    SqlCommand cmd = new SqlCommand("SP_INSERT_Tran_Detail_MSTR_BalanceTransfer", con);    // to be corrected 
                    //    // SqlCommand cmd1 = new SqlCommand("select top 1 * from Customer_IDs where ID_NO ='" + txtCnicNo + "'", oCnn);   
                    //    cmd.CommandType = CommandType.StoredProcedure;
                    //    cmd.Parameters.AddWithValue("@TXNMSTR_VALDATE", Convert.ToDateTime(this.HttpContext.Session["transferdate"]).ToString("yyyy-MM-dd"));

                    //    cmd.Parameters.AddWithValue("@TXNMSTR_POSTDATE", DateTime.Now.Date);
                    //    cmd.Parameters.AddWithValue("@FBACC_ID", Int32.Parse(table.Rows[i]["fbacc_id"].ToString()));
                    //    cmd.Parameters.AddWithValue("@PTRATE_PERCENT", table.Rows[i]["ptrate_percent"].ToString());
                    //    cmd.Parameters.AddWithValue("@TXND_AMOUNT", table.Rows[i]["txnd_amount"].ToString());
                    //    cmd.Parameters.AddWithValue("@TXND_ProfitAMOUNT", table.Rows[i]["profit"].ToString());
                    //    cmd.Parameters.AddWithValue("@sUser", user_session);
                    //    int s_ = cmd.ExecuteNonQuery();
                    //    con.Close();

                    //}

                }
                ///////////////////////////////////    PROCESS FOR NON POOLED BALANCES ////////////////////////////


                using (SqlConnection con = new SqlConnection(constr))
                {
                    con.Open();
                    string QUERY_delete_rpt_balance
                    = "delete from Rpt_BankBalance";
                    using (SqlCommand cmd = new SqlCommand(QUERY_delete_rpt_balance, con))   //replace the stored procedure name with string name defined above
                    {
                        cmd.CommandType = CommandType.Text;
                        int s = cmd.ExecuteNonQuery();
                    }
                    con.Close();

                }

                DataTable table_bal_bank_branch_non_pooled = new DataTable();
                using (SqlConnection con = new SqlConnection(constr))
                {
                    string sum = " insert into Rpt_BankBalance ";
                    sum = sum + "select bank_name as bank,bbr_name as branch,TXND_AMOUNT as balance,fb.FBACC_NUMBER as accountno from STP_FUNDBANKACCOUNT fb";
                    sum = sum + " inner join  TXN_TRANMASTER tm on tm.FBACC_ID= fb.FBACC_ID";
                    sum = sum + " inner join  TXN_TRANDETAIL td on tm.TXNMSTR_ID=td.TXNMSTR_ID";
                    sum = sum + " inner join  stp_bank bnk on bnk.bank_id=fb.bank_id";
                    sum = sum + " inner join  stp_bankbranch br on br.bbr_id=fb.bbr_id";
                    sum = sum + " where  tm.TXNMSTR_STATUS='POST'";
                    sum = sum + "  and td.TXND_STATUS='POST' AND fb.FBACC_ISPOOLED='false' and fb.accpur_id='" + account_pur + "'";
                    sum = sum + " and tm.TXNMSTR_VALDATE='" + check_date + "'";
                   
                    using (SqlCommand cmd = new SqlCommand(sum, con))   //replace the stored procedure name with string name defined above
                    {
                        con.Open();
                        cmd.CommandType = CommandType.Text;
                        int s = cmd.ExecuteNonQuery();
                        //SqlDataAdapter ds = new SqlDataAdapter(cmd);
                        //ds.Fill(table_bal_bank_branch);
                    }
                    con.Close();
                }

                using (SqlConnection con = new SqlConnection(constr))
                {
                    con.Open();
                    string QUERY_delete_rpt_SLAB
                    = "delete from Rpt_SLAB";
                    using (SqlCommand cmd = new SqlCommand(QUERY_delete_rpt_SLAB, con))   //replace the stored procedure name with string name defined above
                    {
                        cmd.CommandType = CommandType.Text;
                        int s = cmd.ExecuteNonQuery();
                    }
                    con.Close();

                }

                DataTable table1_non_pool = new DataTable();
                using (SqlConnection con = new SqlConnection(constr))
                {
                    string QUERY_TABLE1
                    = "select * from Rpt_BankBalance";
                    using (SqlCommand cmd = new SqlCommand(QUERY_TABLE1, con))   //replace the stored procedure name with string name defined above
                    {
                        cmd.CommandType = CommandType.Text;
                        SqlDataAdapter ds = new SqlDataAdapter(cmd);
                        ds.Fill(table1_non_pool);
                    }
                    con.Close();
                }

                for (int i = 0; i < table1_non_pool.Rows.Count; i++)
                {
                    string BAL = "SELECT * FROM STP_BALAMOUNTSLABS S INNER JOIN ( SELECT * FROM rpt_bankbalance R INNER JOIN STP_BANK  BNK ON BNK.BANK_NAME=R.Bank ) RB ";
                    BAL = BAL + " ON S.BANK_ID=RB.BANK_ID";
                    BAL = BAL + " WHERE SLAB_AMOUNTFROM<='" + table1_non_pool.Rows[i]["Balance"].ToString() + "' AND SLAB_AMOUNTTO>'" + table1_non_pool.Rows[i]["Balance"].ToString() + "'";
                    BAL = BAL + " AND BANK_NAME='" + table1_non_pool.Rows[i]["Bank"].ToString() + "'";

                    DataTable table2 = new DataTable();
                    using (SqlConnection con = new SqlConnection(constr))
                    {
                        using (SqlCommand cmd = new SqlCommand(BAL, con))   //replace the stored procedure name with string name defined above
                        {
                            cmd.CommandType = CommandType.Text;
                            SqlDataAdapter ds = new SqlDataAdapter(cmd);
                            ds.Fill(table2);
                        }
                    }
                    if (table2.Rows.Count == 0)
                    {
                        string BAL1 = "SELECT * FROM STP_BALAMOUNTSLABS S INNER JOIN ( SELECT * FROM rpt_bankbalance R INNER JOIN STP_BANK  BNK ON BNK.BANK_NAME=R.Bank ) RB ";
                        BAL1 = BAL1 + " ON S.BANK_ID=RB.BANK_ID";
                        BAL1 = BAL1 + " WHERE SLAB_AMOUNTFROM>='" + table1_non_pool.Rows[i]["Balance"].ToString() + "' AND SLAB_AMOUNTTO<='" + table1_non_pool.Rows[i]["Balance"].ToString() + "'";

                        //DataTable table2 = new DataTable();
                        using (SqlConnection con = new SqlConnection(constr))
                        {
                            using (SqlCommand cmd = new SqlCommand(BAL1, con))   //replace the stored procedure name with string name defined above
                            {
                                cmd.CommandType = CommandType.Text;
                                SqlDataAdapter ds = new SqlDataAdapter(cmd);
                                ds.Fill(table2);
                            }
                        }
                    }

                    for (int j = 0; j < table2.Rows.Count; j++)
                    {

                        using (SqlConnection con = new SqlConnection(constr))
                        {
                            con.Open();
                            // string QUERY_delete_rpt_SLAB
                            string query = "insert into rpt_slab values('" + table2.Rows[j]["Bank"].ToString() + "','" + table2.Rows[j]["Branch"].ToString() + "','" + table2.Rows[j]["SLAB_NAME"].ToString() + "','"+table2.Rows[j]["AccountNo"].ToString()+"')";
                            //Writer insert  query 

                            using (SqlCommand cmd = new SqlCommand(query, con))   //replace the stored procedure name with string name defined above
                            {
                                cmd.CommandType = CommandType.Text;
                                int s = cmd.ExecuteNonQuery();
                            }
                            con.Close();
                        }
                    }

                }

                using (SqlConnection con = new SqlConnection(constr))
                {
                    con.Open();
                    string QUERY_delete_rpt_PROFIT
                    = "delete from Rpt_ProfitRate";
                    using (SqlCommand cmd = new SqlCommand(QUERY_delete_rpt_PROFIT, con))   //replace the stored procedure name with string name defined above
                    {
                        cmd.CommandType = CommandType.Text;
                        int s = cmd.ExecuteNonQuery();
                    }
                    con.Close();
                }

                DataTable table3_non_pool = new DataTable();
                using (SqlConnection con = new SqlConnection(constr))
                {
                    string QUERY_TABLE3
                    = " select distinct * from Rpt_Slab";
                    using (SqlCommand cmd = new SqlCommand(QUERY_TABLE3, con))   //replace the stored procedure name with string name defined above
                    {
                        cmd.CommandType = CommandType.Text;
                        SqlDataAdapter ds = new SqlDataAdapter(cmd);
                        ds.Fill(table3_non_pool);
                    }
                    con.Close();
                }   //////////////////////////////

                if (table3_non_pool.Rows.Count > 0)
                {

                    for (int i = 0; i < table3_non_pool.Rows.Count; i++)
                    {
                        string pfcal = "insert into rpt_profitrate ";
                        pfcal = pfcal + " SELECT bank,branch,ptrate_percent,accountno FROM STP_PROFITRATE PR INNER JOIN ( select * from Rpt_Slab R  INNER JOIN (SELECT BANK_ID BNKID,BANK_NAME BNKNAME FROM STP_BANK)   BNK ON BNK.BNKNAME=R.Bank  INNER JOIN (SELECT BBR_ID BRID, BBR_NAME BRNAME  FROM  STP_BANKBRANCH) BR ON BR.BRNAME=R.BRANCH  INNER JOIN (SELECT SLAB_ID SLBID,SLAB_NAME SLNAME FROM STP_BALAMOUNTSLABS) BLAMNT ON BLAMNT.SLNAME=R.Slab ) S ";
                        pfcal = pfcal + " ON  PR.SLAB_ID=S.SLBID INNER join (select ACCTYPE_ID typeid,ACCTYPE_NAME typename from config_accounttype) acctype on acctype.typeid=pr.acctype_id INNER join (select ACCPUR_ID purid,accpur_name purname from config_accountpurpose) acctpurpose on acctpurpose.purid=pr.ACCPUR_ID   ";
                        pfcal = pfcal + " where  PTRATE_STATUS='ACTIVE' AND  PR.PTRATE_EFFECTIVEFROM <='" + check_date + "' AND PR.PTRATE_EFFECTIVETO>='" + check_date + "'";
                        pfcal = pfcal + " AND BANK='" + table3_non_pool.Rows[i]["Bank"].ToString() + "'" + "AND BRANCH ='" + table3_non_pool.Rows[i]["Branch"].ToString() + "' and slab='" + table3_non_pool.Rows[i]["Slab"].ToString() + "'";// and ACCPUR_ID='" + account_pur + "'";

                        //DataTable table4 = new DataTable();
                        using (SqlConnection con = new SqlConnection(constr))
                        {
                            con.Open();
                            using (SqlCommand cmd = new SqlCommand(pfcal, con))   //replace the stored procedure name with string name defined above
                            {
                                cmd.CommandType = CommandType.Text;
                                int s = cmd.ExecuteNonQuery();
                                //SqlDataAdapter ds = new SqlDataAdapter(cmd);
                                //ds.Fill(table4);
                            }
                            con.Close();
                        }

                    }

                }
                else 
                {
                    ModelState.AddModelError("val_date", "Profit Rate Not Found For This Period");

                    using (SqlConnection con = new SqlConnection(constr))
                    {
                        con.Open();
                        string QUERY_delete_rpt_PROFIT
                        = "delete from Profit_Accrual where valuedate='" + check_date + "'";

                        using (SqlCommand cmd = new SqlCommand(QUERY_delete_rpt_PROFIT, con))   //replace the stored procedure name with string name defined above
                        {
                            cmd.CommandType = CommandType.Text;
                            int s = cmd.ExecuteNonQuery();
                        }
                        con.Close();
                    }

                    using (SqlConnection con = new SqlConnection(constr))
                    {
                        con.Open();
                        string QUERY_delete_rpt_PROFIT
                        = "delete from Process where date='" + check_date + "'";

                        using (SqlCommand cmd = new SqlCommand(QUERY_delete_rpt_PROFIT, con))   //replace the stored procedure name with string name defined above
                        {
                            cmd.CommandType = CommandType.Text;
                            int s = cmd.ExecuteNonQuery();
                        }
                        con.Close();
                    }

                    Process user12 = new Process();
                    return View(user12);
                }

                /////////////////////////

                string serial_non_pool = "";
                DataTable table4_non_pool = new DataTable();
                using (SqlConnection con = new SqlConnection(constr))
                {
                    string QUERY_TABLE4 = "";
                    QUERY_TABLE4 = QUERY_TABLE4 + " select  tm.txnmstr_valdate,fund_name,Bank_name,bbr_name,acctype_name,accpur_name,pbas_name";
                    QUERY_TABLE4 = QUERY_TABLE4 + " ,fbacc_ispooled,pfrq_name,fbacc_number,txnd_amount from STP_FUNDBANKACCOUNT fb ";
                    QUERY_TABLE4 = QUERY_TABLE4 + " inner join  TXN_TRANMASTER tm on tm.FBACC_ID= fb.FBACC_ID ";
                    QUERY_TABLE4 = QUERY_TABLE4 + " inner join  TXN_TRANDETAIL td on tm.TXNMSTR_ID=td.TXNMSTR_ID";
                    QUERY_TABLE4 = QUERY_TABLE4 + " inner join  stp_bank bnk on bnk.bank_id=fb.bank_id";
                    QUERY_TABLE4 = QUERY_TABLE4 + " inner join  stp_bankbranch br on br.bbr_id=fb.bbr_id";
                    QUERY_TABLE4 = QUERY_TABLE4 + " inner join  STP_FUND fund on fund.FUND_ID=fb.FUND_ID";
                    QUERY_TABLE4 = QUERY_TABLE4 + " inner join  CONFIG_ACCOUNTTYPE atype on atype.acctype_id=fb.ACCTYPE_ID";
                    QUERY_TABLE4 = QUERY_TABLE4 + " inner join  CONFIG_ACCOUNTPURPOSE apurpose on apurpose.accpur_id=fb.accpur_id";
                    QUERY_TABLE4 = QUERY_TABLE4 + " inner join  CONFIG_PROFITBASIS pbasis on pbasis.pbas_id=fb.pbas_id";
                    QUERY_TABLE4 = QUERY_TABLE4 + " inner join  CONFIG_PROFITFREQUENCY  pfreq on pfreq.pfrq_id=fb.pfrq_id";
                    QUERY_TABLE4 = QUERY_TABLE4 + " where  tm.TXNMSTR_STATUS='POST' and td.TXND_STATUS='POST' AND fb.FBACC_ISPOOLED='false'  and fb.PFRQ_ID='3'";
                    QUERY_TABLE4 = QUERY_TABLE4 + " and fb.accpur_id='" + account_pur + "' and tm.TXNMSTR_VALDATE='" + check_date + "'";
                    QUERY_TABLE4 = QUERY_TABLE4 + " group by tm.txnmstr_valdate,fund_name,Bank_name,bbr_name,acctype_name,accpur_name,pbas_name";
                    QUERY_TABLE4 = QUERY_TABLE4 + ",fbacc_ispooled,pfrq_name,fbacc_number,txnd_amount ";

                    using (SqlCommand cmd = new SqlCommand(QUERY_TABLE4, con))   //replace the stored procedure name with string name defined above
                    {
                        cmd.CommandType = CommandType.Text;
                        SqlDataAdapter ds = new SqlDataAdapter(cmd);
                        ds.Fill(table4_non_pool);
                    }
                    con.Close();
                }

                for (int i = 0; i < table4_non_pool.Rows.Count; i++)    /////////
                {
                    string query_table5 = "select distinct * from Rpt_ProfitRate ";
                    //query_table5 =query_table5 + " ON S.BANK_ID=RB.BANK_ID";
                    query_table5 = query_table5 + " WHERE BANK='" + table4_non_pool.Rows[i]["Bank_name"].ToString() + "'";
                    query_table5 = query_table5 + " AND BRANCH='" + table4_non_pool.Rows[i]["bbr_name"].ToString() + "'";

                    DataTable table5 = new DataTable();
                    using (SqlConnection con = new SqlConnection(constr))
                    {
                        using (SqlCommand cmd = new SqlCommand(query_table5, con))   //replace the stored procedure name with string name defined above
                        {
                            cmd.CommandType = CommandType.Text;
                            SqlDataAdapter ds = new SqlDataAdapter(cmd);
                            ds.Fill(table5);
                        }
                    }

                    string query_table6 = "select CASE WHEN (Max(Serial) + 1) IS NULL THEN 1 ELSE (Max(Serial) + 1) END as NewId from Profit_Accrual";
                    DataTable table6 = new DataTable();
                    using (SqlConnection con = new SqlConnection(constr))
                    {
                        using (SqlCommand cmd = new SqlCommand(query_table6, con))   //replace the stored procedure name with string name defined above
                        {
                            cmd.CommandType = CommandType.Text;
                            SqlDataAdapter ds = new SqlDataAdapter(cmd);
                            ds.Fill(table6);
                        }
                    }

                    for (int j = 0; j < table5.Rows.Count; j++)
                    {
                        serial_non_pool = table6.Rows[0]["NewId"].ToString();
                        string process_date = DateTime.Now.Date.ToString("yyyy-MM-dd");
                        float Accrual = float.Parse(table4_non_pool.Rows[i]["txnd_amount"].ToString()) * float.Parse(table5.Rows[j]["ProfitRate"].ToString()) / 100;
                        Accrual = Accrual / 365;
                        string username = this.HttpContext.Session["username_session"].ToString();
                        string insert_profit_accural = "";

                        insert_profit_accural = insert_profit_accural + "insert into Profit_Accrual values(@serial,@valudate,@processdate,@fundname,@bankname,@branchname,@type,@purpose,@profitbasis,@pool,@frequency,@accountno,@bankbalance,@profitrate,@accural,@username,@lastbalance)";

                        string ispooled = table4_non_pool.Rows[i]["fbacc_ispooled"].ToString();

                        using (SqlConnection con = new SqlConnection(constr))
                        {
                            con.Open();
                            using (SqlCommand cmd = new SqlCommand(insert_profit_accural, con))   //replace the stored procedure name with string name defined above
                            {
                                string vlue_date = table4_non_pool.Rows[i]["txnmstr_valdate"].ToString();
                                DateTime vlue_date_datetime = Convert.ToDateTime(vlue_date);                                
                                cmd.CommandType = CommandType.Text;
                                cmd.Parameters.AddWithValue("@serial", serial_non_pool);
                               // cmd.Parameters.AddWithValue("@valudate", table4_non_pool.Rows[i]["txnmstr_valdate"].ToString());
                                cmd.Parameters.AddWithValue("@valudate", vlue_date_datetime.ToString("yyyy-MM-dd"));
                                

                                cmd.Parameters.AddWithValue("@processdate", process_date);
                                cmd.Parameters.AddWithValue("@fundname", table4_non_pool.Rows[i]["fund_name"].ToString());
                                cmd.Parameters.AddWithValue("@bankname", table4_non_pool.Rows[i]["Bank_name"].ToString());
                                cmd.Parameters.AddWithValue("@branchname", table4_non_pool.Rows[i]["bbr_name"].ToString());
                                cmd.Parameters.AddWithValue("@type", table4_non_pool.Rows[i]["acctype_name"].ToString());
                                cmd.Parameters.AddWithValue("@purpose", table4_non_pool.Rows[i]["accpur_name"].ToString());
                                cmd.Parameters.AddWithValue("@profitbasis", table4_non_pool.Rows[i]["pbas_name"].ToString());
                                cmd.Parameters.AddWithValue("@pool", ispooled);
                                cmd.Parameters.AddWithValue("@frequency", table4_non_pool.Rows[i]["pfrq_name"].ToString());
                                cmd.Parameters.AddWithValue("@accountno", table4_non_pool.Rows[i]["fbacc_number"].ToString());
                                cmd.Parameters.AddWithValue("@bankbalance", table4_non_pool.Rows[i]["txnd_amount"].ToString());
                                cmd.Parameters.AddWithValue("@profitrate", table5.Rows[j]["ProfitRate"].ToString());
                                cmd.Parameters.AddWithValue("@accural", Accrual.ToString());
                                cmd.Parameters.AddWithValue("@username", username);
                               // cmd.Parameters.AddWithValue("@lastbalance", table4_non_pool.Rows[i]["txnd_amount"].ToString());

                                cmd.Parameters.AddWithValue("@lastbalance", DBNull.Value);

                                int s = cmd.ExecuteNonQuery();
                                //SqlDataAdapter ds = new SqlDataAdapter(cmd);
                                //ds.Fill(table4);
                            }
                            con.Close();
                        }
                    }

                }

                using (SqlConnection con = new SqlConnection(constr))
                {
                    con.Open();

                    string purpose = "";

                    if (model.Purpose.ToString() != "NORMAL")
                    {
                   purpose = "INCOME";

                    }
                    else
                    {
                    //    account_pur = "1";
                        purpose = "NORMAL";                    
                    }
                    
                    
                    string QUERY_delete_rpt_PROFIT
                    = "insert into process values('DAILY','"+check_date+"','"+purpose+"')";

                    using (SqlCommand cmd = new SqlCommand(QUERY_delete_rpt_PROFIT, con))   //replace the stored procedure name with string name defined above
                    {
                        cmd.CommandType = CommandType.Text;
                        int s = cmd.ExecuteNonQuery();
                    }
                    con.Close();
                }

                ViewBag.Message = string.Format("Process Completed Successfully");

                this.HttpContext.Session["message_Process_successful"] = "Process Completed Successfully";


            }
          
          
          Process user1 = new Process();
          return View(user1);


        }

        public ActionResult Profit_Accrual_Report() 
        {
            if (this.HttpContext.Session["username_session"] == null)
            {
                return RedirectToAction("login", "Home");
            }            
            Profit_Accrual_Report user1 = new Profit_Accrual_Report();
            return View(user1);        
        }
        
        [HttpPost]
        public ActionResult Profit_Accrual_Report(Profit_Accrual_Report model) 
        {


            Profit_Accrual_Report user1 = new Profit_Accrual_Report();
           // user1.post_date_string = Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd");
            return View(user1);        
      
        
        
        }

        public ActionResult ExportCrystalReport(Profit_Accrual_Report model)
        {
            string constr = ConfigurationManager.ConnectionStrings["DAP_NEW"].ConnectionString;
            model.search_option = Request.Form["wise"].ToString();
            this.HttpContext.Session["option_session_report"] = model.search_option.ToString();
            string opt_session = this.HttpContext.Session["option_session_report"].ToString();

            string purpose = model.Purpose;
            string startdate= model.StartDate.ToString("yyyy-MM-dd");
            string enddate = model.EndDate.ToString("yyyy-MM-dd");

            DateTime date_start = (Convert.ToDateTime(startdate.ToString()));
            DateTime date_end = (Convert.ToDateTime(enddate.ToString()));
            String start_dd = date_start.Day.ToString();
            String start_mn = date_start.Month.ToString();
            String start_yy = date_start.Year.ToString();
            String end_dd = date_end.Day.ToString();
            String end_mn = date_end.Month.ToString();
            String end_yy = date_end.Year.ToString();

         //   ssql = ssql & " and {Profit_Accrual.ValueDate} >= date(" & Year(.txtStartDate) & "," & Month(.txtStartDate) & "," & Day(.txtStartDate) & ") and  {Profit_Accrual.ValueDate} <= date(" & Year(.txtEndDate) & "," & Month(.txtEndDate) & "," & Day(.txtEndDate) & ")"


   string main_report_query = "select * from Profit_Accrual";
   main_report_query = main_report_query + " where valuedate>='" + startdate + "' and  valuedate<='" + enddate + "'";
            DataTable table1 = new DataTable();
            using (SqlConnection con = new SqlConnection(constr))
            {
              
//                select * from Profit_Accrual"
//ssql = ssql & " where ValueDate >= " & "'" & Format(txtStartDate, "yyyy/MM/dd") & "'"
//ssql = ssql & " and ValueDate <= " & "'" & Format(txtEndDate, "yyyy/MM/dd") & "'"
                                
                string QUERY_TABLE1= "select * from Profit_Accrual";                
                QUERY_TABLE1=QUERY_TABLE1 +" where valuedate>='"+startdate+"' and  valuedate<='"+enddate+"'";                
                using (SqlCommand cmd = new SqlCommand(QUERY_TABLE1, con))   //replace the stored procedure name with string name defined above
                {
                    cmd.CommandType = CommandType.Text;
                    SqlDataAdapter ds = new SqlDataAdapter(cmd);
                    ds.Fill(table1);
                }
                con.Close();
            }////

            if (table1.Rows.Count > 0)
            {
                for (int i = 0; i < table1.Rows.Count; i++)
                {
                    DateTime pdate = DateTime.Parse(table1.Rows[i]["valuedate"].ToString()).AddDays(-1);
                    string prevdate = pdate.ToString("yyyy-MM-dd");

//                    select * from txn_trandetail 
//where 
//txnmstr_id in (select TXNMSTR_ID from TXN_TRANMASTER where TXNMSTR_VALDATE='2019-11-14')
//and FBACC_ID in (select distinct FBACC_ID from stp_fundbankaccount where fbacc_number='0100019848')


string QUERY_TABLE2 = " select * from txn_trandetail  ";
QUERY_TABLE2 = QUERY_TABLE2 + " where txnmstr_id in (select TXNMSTR_ID from TXN_TRANMASTER where TXNMSTR_VALDATE='" +prevdate+ "')";
QUERY_TABLE2 = QUERY_TABLE2 + " and FBACC_ID in (select distinct FBACC_ID from stp_fundbankaccount where fbacc_number='" + table1.Rows[i]["AccountNo"].ToString() + "')";

DataTable table2 = new DataTable();
using (SqlConnection con = new SqlConnection(constr))
{
    using (SqlCommand cmd = new SqlCommand(QUERY_TABLE2, con))   //replace the stored procedure name with string name defined above
    {
        cmd.CommandType = CommandType.Text;
        SqlDataAdapter ds = new SqlDataAdapter(cmd);
        ds.Fill(table2);
    }
    con.Close();
}

if (table2.Rows.Count > 0) 
{ 
    for(int c=0 ;c<table2.Rows.Count;c++)
    {

  using (SqlConnection con = new SqlConnection(constr))
  {
      con.Open();

      string vlue_date = table1.Rows[i]["valuedate"].ToString();
      DateTime vlue_date_datetime = Convert.ToDateTime(vlue_date);
      
      string query_3 = "update profit_accrual set lastbalance='" + table2.Rows[c]["txnd_amount"].ToString()
       + "' where  valuedate='" + vlue_date_datetime.ToString("yyyy-MM-dd")  +"' and accountno='" + table1.Rows[i]["accountno"] + "'";

      using (SqlCommand cmd = new SqlCommand(query_3, con))   //replace the stored procedure name with string name defined above
      {
          cmd.CommandType = CommandType.Text;
          int s = cmd.ExecuteNonQuery();
      }
      con.Close();
  }

    }

}

                }

            }


            main_report_query = main_report_query + " and Frequency ='DAILY' ";         
              string    ssql = "{Profit_Accrual.Frequency} = '" + "Daily" + "'";             
            if (opt_session == "Bank")
            {
                main_report_query = main_report_query + " and Type = 'SAVING'";
                ssql = ssql + " and {Profit_Accrual.Type} = 'SAVING'";
            }
            if (opt_session == "Account")
            {
                main_report_query = main_report_query + " and AccountNo  ='" + model.FBACC_NUMBER.ToString()+"'";
                ssql = ssql + " and {Profit_Accrual.AccountNo} = '" + model.FBACC_NUMBER.ToString() + "'";            
            }

//ssql = ssql & " and {Profit_Accrual.ValueDate} >= date(" & Year(.txtStartDate) & "," & Month(.txtStartDate) & "," & Day(.txtStartDate) & ") and  {Profit_Accrual.ValueDate} <= date(" & Year(.txtEndDate) & "," & Month(.txtEndDate) & "," & Day(.txtEndDate) & ")"

          ssql = ssql + " and {Profit_Accrual.ValueDate} >= date("+ start_yy + "," +start_mn + "," + start_dd +") and  {Profit_Accrual.ValueDate} <= date(" + end_yy + "," + end_mn + "," + end_dd +")";
          main_report_query = main_report_query + " and Purpose = '" + model.Purpose.ToString() + "'";
          ssql = ssql + " and {Profit_Accrual.Purpose} = '" + model.Purpose + "'";

            ReportDocument rd = new ReportDocument();
            if (opt_session == "Fund")
            {
                string strRptLoad = Server.MapPath("~/Reports/ProfitAccrual_Fund.rpt");                  
               // rd.Load(Path.Combine(Server.MapPath("~/Reports/"), "ProfitAccrual_Fund.rpt"));
                rd.Load(strRptLoad);
             ///   Server.MapPath("/Reports/MyReport.rpt"); 
            }
            if(opt_session == "Bank")
            {
                string strRptLoad = Server.MapPath("~/Reports/ProfitAccrual.rpt");  
            //    rd.Load(Path.Combine(Server.MapPath("~/Reports/"), "ProfitAccrual.rpt"));
                rd.Load(strRptLoad);
            }

          //  report.DataSourceConnections[0].SetConnection(_servername, _databasename, _userid, _password);
            //rd.DataDefinition.FormulaFields["FromDate"].Text = "'" + startdate + "'";            
            //rd.DataDefinition.FormulaFields["ToDate"].Text = "'" + enddate + "'";


            string serverName = ConfigurationManager.AppSettings["server_name"];
            string dbName = ConfigurationManager.AppSettings["db_name"];
            string userName = ConfigurationManager.AppSettings["user_name"];
            string pword = ConfigurationManager.AppSettings["password_"];


            rd.DataSourceConnections[0].SetConnection(serverName,dbName, userName,pword);
            rd.DataDefinition.RecordSelectionFormula = ssql;
            rd.DataDefinition.FormulaFields["FromDate"].Text= "'" + enddate + "'";
            rd.DataDefinition.FormulaFields["ToDate"].Text = "'" + startdate + "'";
            rd.DataDefinition.FormulaFields["Purpose"].Text = "'" + purpose + "'";

           
            //CrystalDecisions.Shared.TableLogOnInfo li = new CrystalDecisions.Shared.TableLogOnInfo();
            //li.ConnectionInfo.IntegratedSecurity = false;
            MemoryStream oStream;
            oStream = (MemoryStream)
                       rd.ExportToStream(
                       CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);

            Response.Clear();
            Response.Buffer = true;
            Response.ContentType = "Application/pdf";
          //  Response.AddHeader("content-disposition", "inline ;filename=Profit_Accrual_report.pdf" + ".pdf");
            Response.BinaryWrite(oStream.ToArray());

            //////

            //oStream.Flush();
            //oStream.Close();
            //oStream.Dispose();
            ////////////

            //Response.Buffer = false;
            //Response.ClearContent();
            //Response.ClearHeaders();
            //Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            //stream.Seek(0, SeekOrigin.Begin);

           // oStream.Close();
            //oStream.Dispose();
            // File(oStream, "application/pdf", "Profit_Accrual_report.pdf");

            rd.Close();
            rd.Dispose();
            SqlConnection.ClearAllPools();



            return File(oStream, "application/pdf", "Profit_Accrual_report.pdf");        
        }

        public ActionResult Bank_Balance_Report()
        {

            if (this.HttpContext.Session["username_session"] == null)
            {
                return RedirectToAction("login", "Home");
            }
            
            
            Bank_Balance_Report user1 = new Bank_Balance_Report();
            return View(user1);
        }

        public ActionResult ExportCrystalReport_BANKBALANCE(Profit_Accrual_Report model)
        {
            // List<Customer> allCustomer = new List<Customer>();
            // allCustomer = context.Customers.ToList();

            model.search_option = Request.Form["wise"].ToString();

            this.HttpContext.Session["option_session_report"] = model.search_option.ToString();
            string opt_session = this.HttpContext.Session["option_session_report"].ToString();
            string purpose = model.Purpose;
            string startdate = model.StartDate.ToString("yyyy-MM-dd");
            string enddate = model.EndDate.ToString("yyyy-MM-dd");

            DateTime date_start = (Convert.ToDateTime(startdate.ToString()));
            DateTime date_end = (Convert.ToDateTime(enddate.ToString()));

            String start_dd = date_start.Day.ToString();
            String start_mn = date_start.Month.ToString();
            String start_yy = date_start.Year.ToString();

            String end_dd = date_end.Day.ToString();
            String end_mn = date_end.Month.ToString();
            String end_yy = date_end.Year.ToString();
            // ssql = ssql + " and {Profit_Accrual.ValueDate} >= date("+ start_yy + "," +start_mn + "," + start_dd +") and  {Profit_Accrual.ValueDate} <= date(" + end_yy + "," + end_mn + "," + end_dd +")";
            string ssql = "{BankBalances.ValueDate} >= date(" + start_yy + "," + start_mn + "," + start_dd + ") and  {BankBalances.ValueDate} <= date(" + end_yy + "," + end_mn + "," + end_dd + ")";


            ReportDocument rd = new ReportDocument();
            if (opt_session == "Fund")
            {   
                string strRptLoad = Server.MapPath("~/Reports/BankBalances.rpt");
                rd.Load(strRptLoad);
            
            }
            if (opt_session == "Bank")
            {
                string strRptLoad = Server.MapPath("~/Reports/BankBalances.rpt");            
                rd.Load(strRptLoad);
            }
            if (opt_session == "All")
            {
                string strRptLoad = Server.MapPath("~/Reports/BankBalances.rpt");
                rd.Load(strRptLoad);
            }

            string serverName = ConfigurationManager.AppSettings["server_name"];
            string dbName = ConfigurationManager.AppSettings["db_name"];
            string userName = ConfigurationManager.AppSettings["user_name"];
            string pword = ConfigurationManager.AppSettings["password_"];

            rd.DataSourceConnections[0].SetConnection(serverName, dbName, userName, pword);
            rd.DataDefinition.RecordSelectionFormula = ssql;

            MemoryStream oStream;
            oStream = (MemoryStream)
                       rd.ExportToStream(
                       CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);

            Response.Clear();
            Response.Buffer = true;
            Response.ContentType = "Application/pdf";
            //Response.AddHeader("content-disposition", "inline ;filename=Profit_Accrual_report.pdf" + ".pdf");
            Response.BinaryWrite(oStream.ToArray());

            //Response.Buffer = false;
            //Response.ClearContent();
            //Response.ClearHeaders();

            //Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            //stream.Seek(0, SeekOrigin.Begin);
            return File(oStream, "application/pdf", "Bank_Balance_Report.pdf");
  
        
        }


    }
    
}