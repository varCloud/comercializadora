using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Configuration;

namespace lluviaBackEnd.Utilerias
{
    public class Email
    {
        #region Notificación recuperar contraseña
        public static void NotificacionRecuperarContrasena(string emailDestino, string contrasena)
        {
            try
            {
                string cuerpo = Cabecera();
                cuerpo += CuerpoNotificacion(contrasena);
                cuerpo += PiePagina();
                //EnviarCorreoExternoUsuario("Recuperar contraseña", cuerpo, emailDestino);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void NotificacionPagoReferencia(string emailDestino, string pathArchivo)
        {
            try
            {
                string cuerpo = Cabecera();
                cuerpo += CuerpoNotificacionPagoReferencia();
                cuerpo += PiePagina();
                EnviarCorreoExternoUsuario("Factura Comercializadora Lluvia", cuerpo, emailDestino , pathArchivo);
                /*
                string cuerpo2 = Cabecera();
                cuerpo2 += CuerpoNotificacionPagoConsola(referencia, idOrdenCompra);
                cuerpo2 += PiePagina();
                EnviarCorreoExternoUsuario("Pago recibido", cuerpo2, "ecommerce@crea3m.com");
                */

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static string CuerpoNotificacion(string contrasena)
        {
            StringBuilder cuerpo = new StringBuilder();
            cuerpo.Append(@"<table border='0' cellpadding='0' cellspacing='0' width='100%'>	
	            <tr>
		            <td style='padding: 10px 0 30px 0;'>
			            <table align='center' border='0' cellpadding='0' cellspacing='0' width='600' style='border: 1px solid #cccccc; border-collapse: collapse;'>
				            <tr>
					            <td bgcolor='#ffffff' style='padding: 40px 30px 40px 30px;'>
						            <table border='0' cellpadding='0' cellspacing='0' width='100%'>
							            <tr>
								            <td style='color: #153643; font-family: Arial, sans-serif; font-size: 24px;'>
									            <b>" + "Recuperar contraseña" + @"!</b>
								            </td>
							            </tr>
							            <tr>
								            <td style='padding: 20px 0 30px 0; color: #153643; font-family: Arial, sans-serif; font-size: 16px; line-height: 20px;'>
									            Estimado usuario te enviamos la contraseña para poder <a href='http://tienda.crea3m.com/#/login' style='color: #007bff;'><font >iniciar sesion</font></a>:
								            </td>
							            </tr>
							            <tr>
								            <td>
									            <table border='0' cellpadding='0' cellspacing='0' width='100%'>
										            <tr>
											            <td width='260' valign='top'>
												            <table border='0' cellpadding='0' cellspacing='0' width='100%'>
													            <tr>
														            <td style='padding: 25px 0 0 0; color: #153643; font-family: Arial, sans-serif; font-size: 14px; line-height: 20px;'>
															            <table border='0' cellpadding='0' cellspacing='0' width='100%'>
															            <tr>
																            <td>
                                                                                <b>Contraseña: </b>" + contrasena + @"
																            </td>
															            </tr>
															            <tr><td><br/></td></tr>
															            <tr>
																            <td>
																	            <b>Fecha: </b>" + DateTime.Now.ToString("MM/dd/yyyy h:mm tt") + @"
	                                                                        </td>
															            </tr>
	                                                    	            </table>
														            </td>
													            </tr>
												            </table>
											            </td>
											            <td style='font-size: 0; line-height: 0;' width='20'>
												            &nbsp;
											            </td>
										            </tr>
									            </table>
								            </td>
							            </tr>
						            </table>
					            </td>
				            </tr>
				            <tr>
					            <td bgcolor='#f52121' style='padding: 30px 30px 30px 30px;'>
						            <table border='0' cellpadding='0' cellspacing='0' width='100%'>
							            <tr style='text-align: center;'>
								            <td style='color: #ffffff; font-family: Arial, sans-serif; font-size: 14px; width='75%'>
									            &reg; Copyright © 2019. CREA <br/>
									            <a href='http://www.crea3m.com/' style='color: #ffffff;'><font color='#ffffff'>CREA</font></a>
								            </td>
							            </tr>
						            </table>
					            </td>
				            </tr>
			            </table>
		            </td>
	            </tr>
            </table>");
            return cuerpo.ToString();
        }


        private static string CuerpoNotificacionPagoReferencia()
        {
            StringBuilder cuerpo = new StringBuilder();
            cuerpo.Append(@"<table border='0' cellpadding='0' cellspacing='0' width='100%'>	
	            <tr>
		            <td style='padding: 10px 0 30px 0;'>
			            <table align='center' border='0' cellpadding='0' cellspacing='0' width='600' style='border: 1px solid #cccccc; border-collapse: collapse;'>
				            <tr>
					            <td bgcolor='#ffffff' style='padding: 40px 30px 40px 30px;'>
						            <table border='0' cellpadding='0' cellspacing='0' width='100%'>
							            <tr>
								            <td style='color: #153643; font-family: Arial, sans-serif; font-size: 24px;'>
									            <b>" + "¡Factura!" + @"!</b>
								            </td>
							            </tr>
							            <tr>
								            <td style='padding: 20px 0 30px 0; color: #153643; font-family: Arial, sans-serif; font-size: 16px; line-height: 20px;'>
									            Gracias por comprar con nosotros, te esperamos pronto...!!
								            </td>
							            </tr>
							            <tr>
								            <td>
									            <table border='0' cellpadding='0' cellspacing='0' width='100%'>
										            <tr>
											            <td width='260' valign='top'>
												            <table border='0' cellpadding='0' cellspacing='0' width='100%'>
													            <tr>
														            <td style='padding: 25px 0 0 0; color: #153643; font-family: Arial, sans-serif; font-size: 14px; line-height: 20px;'>
															            <table border='0' cellpadding='0' cellspacing='0' width='100%'>
															            <tr>
																            <td>
                                                                                <b># Ticket: </b>" + DateTime.Now.ToString("MM/dd/yyyy h:mm tt") + @"
																            </td>
															            </tr>
                                                                        <tr>
																            <td>
                                                                             
																            </td>
															            </tr>
															            <tr><td><br/></td></tr>
															            <tr>
																            <td>
																	            <b>Fecha: </b>" + DateTime.Now.ToString("MM/dd/yyyy h:mm tt") + @"
	                                                                        </td>
															            </tr>
	                                                    	            </table>
														            </td>
													            </tr>
												            </table>
											            </td>
											            <td style='font-size: 0; line-height: 0;' width='20'>
												            &nbsp;
											            </td>
										            </tr>
									            </table>
								            </td>
							            </tr>
						            </table>
					            </td>
				            </tr>
				            <tr>
					            <td bgcolor='#a6ce3a' style='padding: 30px 30px 30px 30px;'>
						            <table border='0' cellpadding='0' cellspacing='0' width='100%'>
							            <tr style='text-align: center;'>
								            <td style='color: #ffffff; font-family: Arial, sans-serif; font-size: 14px; width='75%'>
									            &reg; Copyright © 2019. Comercializadora Lluvia <br/>
									            <a href='http://comercializadoralluvia.com/' style='color: #ffffff;'><font color='#ffffff'>Comercializadora LLuvia</font></a>
								            </td>
							            </tr>
						            </table>
					            </td>
				            </tr>
			            </table>
		            </td>
	            </tr>
            </table>");
            return cuerpo.ToString();
        }

        private static string CuerpoNotificacionPagoConsola(string referencia, string idOrden)
        {
            StringBuilder cuerpo = new StringBuilder();
            cuerpo.Append(@"<table border='0' cellpadding='0' cellspacing='0' width='100%'>	
	            <tr>
		            <td style='padding: 10px 0 30px 0;'>
			            <table align='center' border='0' cellpadding='0' cellspacing='0' width='600' style='border: 1px solid #cccccc; border-collapse: collapse;'>
				            <tr>
					            <td bgcolor='#ffffff' style='padding: 40px 30px 40px 30px;'>
						            <table border='0' cellpadding='0' cellspacing='0' width='100%'>
							            <tr>
								            <td style='color: #153643; font-family: Arial, sans-serif; font-size: 24px;'>
									            <b>" + "¡Pago Éxitoso!" + @"!</b>
								            </td>
							            </tr>
							            <tr>
								            <td style='padding: 20px 0 30px 0; color: #153643; font-family: Arial, sans-serif; font-size: 16px; line-height: 20px;'>
									            Estimado usuario, se ha realizado el pago por parte de uno de sus clientes, correspondiente a la referencia que se menciona a continuación, favor de continuar con el proceso de la preparación de su pedido.
								            </td>
							            </tr>
							            <tr>
								            <td>
									            <table border='0' cellpadding='0' cellspacing='0' width='100%'>
										            <tr>
											            <td width='260' valign='top'>
												            <table border='0' cellpadding='0' cellspacing='0' width='100%'>
													            <tr>
														            <td style='padding: 25px 0 0 0; color: #153643; font-family: Arial, sans-serif; font-size: 14px; line-height: 20px;'>
															            <table border='0' cellpadding='0' cellspacing='0' width='100%'>
															            <tr>
																            <td>
                                                                                <b>Referencia pagada: </b>" + referencia + @"
																            </td>
															            </tr>
															            <tr>
																            <td>
                                                                                 <b>Id Orden: </b>" + idOrden + @"
																            </td>
															            </tr>
															            <tr><td><br/></td></tr>
															            <tr>
																            <td>
																	            <b>Fecha: </b>" + DateTime.Now.ToString("MM/dd/yyyy h:mm tt") + @"
	                                                                        </td>
															            </tr>
	                                                    	            </table>
														            </td>
													            </tr>
												            </table>
											            </td>
											            <td style='font-size: 0; line-height: 0;' width='20'>
												            &nbsp;
											            </td>
										            </tr>
									            </table>
								            </td>
							            </tr>
						            </table>
					            </td>
				            </tr>
				            <tr>
					            <td bgcolor='#f52121' style='padding: 30px 30px 30px 30px;'>
						            <table border='0' cellpadding='0' cellspacing='0' width='100%'>
							            <tr style='text-align: center;'>
								            <td style='color: #ffffff; font-family: Arial, sans-serif; font-size: 14px; width='75%'>
									            &reg; Copyright © 2019. CREA <br/>
									            <a href='http://www.crea3m.com/' style='color: #ffffff;'><font color='#ffffff'>CREA</font></a>
								            </td>
							            </tr>
						            </table>
					            </td>
				            </tr>
			            </table>
		            </td>
	            </tr>
            </table>");
            return cuerpo.ToString();
        }

        #endregion Notificación recuperar contraseña

        #region FUNCIONES - PRIVADAS
        private static string Cabecera()
        {
            return @"<html><body><!DOCTYPE html>
            <html>
            <head>
	            <title></title>
            </head>
            <body>";
        }

        private static string Cuerpo(string mensaje)
        {
            string cuerpo = "";
            cuerpo += @"<h3>" + mensaje + "</h3>";
            return cuerpo;
        }

        private static String PiePagina()
        {
            StringBuilder pie = new StringBuilder();
            pie.Append("<br /><br />");
            pie.Append("</body>");
            pie.Append("</html>");
            return pie.ToString();
        }

        private static void EnviarCorreoExternoUsuario(string asunto, string cuerpo, string emailUSuario, string file)
        {
            try
            {
                string correoProveedor = WebConfigurationManager.AppSettings["correoProveedor"].ToString();//"cristian.perez.garcia.54@gmail.com";
                string contrasenaProveedor = WebConfigurationManager.AppSettings["contrasenaProveedor"].ToString(); //ConfigurationManager.AppSettings["contrasenaCorreoExterno"].ToString(); //"Kaneki_54";

                System.Net.Mail.MailMessage mmsg = new System.Net.Mail.MailMessage();
                mmsg.To.Add(emailUSuario); // cuenta Email a la cual sera dirigido el correo
                mmsg.Bcc.Add("sapitopicador@gmail.com");
                mmsg.Bcc.Add("graciela.guizar @gmail.com");
                mmsg.Subject = asunto; //Asunto del correo
                mmsg.SubjectEncoding = System.Text.Encoding.UTF8; //cambiamos el tipo de texto a UTF8
                mmsg.Body = cuerpo; //Cuerpo del mensaje
                mmsg.BodyEncoding = System.Text.Encoding.UTF8; // tambien encodear a utf8
                mmsg.IsBodyHtml = true; // indicamos que dentro del body viene codigo HTML
                mmsg.From = new System.Net.Mail.MailAddress(correoProveedor); // el email que enviara el correo (proveedor)
                mmsg.Attachments.Add(new Attachment(file.Replace("Timbre_","Factura_").Replace("xml","pdf")));
                mmsg.Attachments.Add(new Attachment(file));
                System.Net.Mail.SmtpClient cliente = new System.Net.Mail.SmtpClient(); // se realiza el cliente correo

                cliente.Port = 587;
                cliente.EnableSsl = true;
                cliente.UseDefaultCredentials = false;
                cliente.DeliveryMethod = SmtpDeliveryMethod.Network;
                cliente.Host = "smtp.gmail.com"; //mail.dominio.com
                cliente.Credentials = new System.Net.NetworkCredential(correoProveedor, contrasenaProveedor);  // Credenciales del correo emisor
                //smtp
                cliente.Send(mmsg);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion FUNCIONES - PRIVADAS
    }

}