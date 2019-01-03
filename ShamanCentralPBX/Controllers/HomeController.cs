using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

namespace ShamanCentralPBX.Controllers
{
    public class HomeController : Controller
    {
        static string connetionString = ConfigurationManager.AppSettings["MySqlConnetionString"];
        static int minuteVariation = Convert.ToInt32(ConfigurationManager.AppSettings["MinuteVariation"]);

        //public IActionResult Index()
        //{
        //    return View();
        //}
        //https://localhost:44360/5?fechaHoraLlamado=10000&numero=20000&agente=30000
        [HttpGet("{uniqueId}")]
        public IActionResult Index(string uniqueId, string fechaHoraLlamado, string numero, string agente)
        {

            //app.UseStaticFiles(new StaticFileOptions()
            //{
            //    FileProvider = new PhysicalFileProvider(
            //    Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot", "files")),
            //    RequestPath = new PathString("/MyFyles")
            //});


            string callFileName = "https://www.computerhope.com/jargon/m/example.mp3?" + DateTime.Now.Millisecond;

            callFileName = GetFileByUniqueId(uniqueId);

            if (string.IsNullOrEmpty(callFileName))
                callFileName = GetFileByPoperties(fechaHoraLlamado, numero, agente);

            ViewData["callFileName"] = callFileName;
            //Response.Redirect(fileToPlay, true);
            return View(); //uniqueId + " " + fechaHoraLlamado + " " + numero + " " + agente;
        }

        private string GetFileByPoperties(string fechaHoraLlamado, string numero, string agente)
        {
            if (string.IsNullOrEmpty(fechaHoraLlamado) &&
                string.IsNullOrEmpty(numero) &&
                string.IsNullOrEmpty(agente)) return null;

            try
            {
                using (MySqlConnection cnn = new MySqlConnection(connetionString))
                {
                    using (MySqlCommand cmd = cnn.CreateCommand())
                    {
                        cnn.Open();

                        cmd.CommandText = "select CALLFILENAME" +
                                            "from cdr" +
                                            "where calldate <= " + Convert.ToDateTime(fechaHoraLlamado).AddMinutes(-minuteVariation).ToString() +
                                            "and calldate >= " + Convert.ToDateTime(fechaHoraLlamado).AddMinutes(minuteVariation).ToString() +
                                            "and src = " + numero +
                                            "and agente = " + agente;

                        using (MySqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            DataTable dt = new DataTable();
                            dt.Load(rdr);

                            if (dt.Rows.Count > 0)
                            {
                                return dt.Rows[0][0].ToString();
                            }
                            //else
                            //    addLog(false, "ReadMySqlRings", "No hay llamadas entrantes");
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                //addLog(false, "ReadMySqlRings", ex.Message);
            }
            return null;
        }

        private string GetFileByUniqueId(string uniqueId)
        {
            if(string.IsNullOrEmpty(uniqueId)) return null;

            try
            {
                using (MySqlConnection cnn = new MySqlConnection(connetionString))
                {
                    using (MySqlCommand cmd = cnn.CreateCommand())
                    {
                        cnn.Open();

                        cmd.CommandText = "select CALLFILENAME" +
                                          "from cdr where uniqueid = " + uniqueId;

                        using (MySqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            DataTable dt = new DataTable();
                            dt.Load(rdr);

                            if (dt.Rows.Count > 0)
                            {
                                return dt.Rows[0][0].ToString();
                            }
                            //else
                            //    addLog(false, "ReadMySqlRings", "No hay llamadas entrantes");
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                //addLog(false, "ReadMySqlRings", ex.Message);
            }
            return null;
        }
    }
}