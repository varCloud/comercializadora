using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace lluviaBackEnd.Utilerias
{
    public class ItextEvents : PdfPageEventHelper
    {

        private string Titulo = "style='text-align:left; font-size:12px font-weight:bold;'";
        private string derecha = "style='text-align:right;'";

        public string pathLogo { get; set; }
        public string pathMarcaAgua { get { return _pathMarcaAgua; } set { _pathMarcaAgua = value; } }
        private string cssTabla = @"style='text-align:center;font-size:8px;font-family:Arial'";
        private int tamaño = 8;
        string _pathMarcaAgua = "";
        public string TituloCabecera { get; set; }




        // This is the contentbyte object of the writer
        PdfContentByte cb;

        // we will put the final number of pages in a template
        PdfTemplate headerTemplate, template;

        // this is the BaseFont we are going to use for the header / footer
        BaseFont bf = null;

        // This keeps track of the creation time
        DateTime PrintTime = DateTime.Now;


        #region Fields
        private string _header;
        #endregion

        #region Properties
        public string Header
        {
            get { return _header; }
            set { _header = value; }
        }
        #endregion

        public ItextEvents()
        {
           this.pathLogo=  Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "assets","img", "logo_lluvia.png");
           this.pathMarcaAgua = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "assets", "img", "marcaAgua.png");
        }

        public override void OnOpenDocument(PdfWriter writer, Document document)
        {
            try
            {
                bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                cb = writer.DirectContent;
                template = cb.CreateTemplate(50, 50);
            }
            catch (DocumentException de)
            {

            }
            catch (System.IO.IOException ioe)
            {

            }
        }

        public override void OnEndPage(iTextSharp.text.pdf.PdfWriter writer, iTextSharp.text.Document document)
        {
            try
            {
                base.OnEndPage(writer, document);
                String text = "Página " + writer.PageNumber + " de ";
                float len = bf.GetWidthPoint(text, tamaño);
                Rectangle pageSize = document.PageSize;
                cb.SetRGBColorFill(100, 100, 100);
                cb.BeginText();
                cb.SetFontAndSize(bf, tamaño);
                cb.SetTextMatrix(pageSize.GetRight(85), pageSize.GetBottom(65));
                cb.ShowText(text);
                cb.EndText();
                cb.AddTemplate(template, pageSize.GetRight(85) + len, pageSize.GetBottom(65));

                cb.BeginText();
                cb.SetFontAndSize(bf, tamaño);
                cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "",
                    pageSize.GetLeft(30),
                    pageSize.GetBottom(65), 0);
                cb.EndText();

                cb.BeginText();
                cb.SetFontAndSize(bf, tamaño);
                cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "",
                    pageSize.GetLeft(30),
                    pageSize.GetBottom(55), 0);
                cb.EndText();


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public override void OnStartPage(PdfWriter writer, Document document)
        {
            string encabezado = @"<br/><html><body>
                <table border='0' width='100%'" + cssTabla + @"   CELLPADDING='0'>
                    <tr>
                        <td> 
                            <img src='" + pathLogo+ @"' width = '100' height = '70' />
                        </td>
                        <td "+ Titulo + @">
                                Articulos de limpieza lluvia
                        </td>
                    <tr>
                        <td   width='40%' " + derecha + @"></td>
                        <td   width='60%' " + derecha + @"></td>
                    </tr>
                    </tr>
                </table>";

            foreach (IElement E in HTMLWorker.ParseToList(new StringReader(encabezado.ToString()), new StyleSheet()))
            {
                document.Add(E);
            }
            AddWaterMark(document);
        }

        public override void OnCloseDocument(PdfWriter writer, Document document)
        {
            try
            {
                base.OnCloseDocument(writer, document);
                template.BeginText();
                template.SetFontAndSize(bf, tamaño);
                template.SetTextMatrix(0, 0);
                if(writer.PageNumber == 1)
                    template.ShowText("" + writer.PageNumber  );
                else
                    template.ShowText("" + (writer.PageNumber - 1));
                template.EndText();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public  void AddWaterMark(Document objPdfDocument)
        {
            try
            {

               
                Image objImagePdf;

                //    //// Crea la imagen
                objImagePdf = Image.GetInstance(_pathMarcaAgua);
                //    //// Cambia el tamaño de la imagen
                //objImagePdf.ScalePercent(45, 45);
            
                //    //// Se indica que la imagen debe almacenarse como fondo
                objImagePdf.Alignment = iTextSharp.text.Image.UNDERLYING;
                //    //// Coloca la imagen en una posición absoluta
                objImagePdf.SetAbsolutePosition((objPdfDocument.PageSize.Width / 4), (objPdfDocument.PageSize.Height / 4));
                //    //// Imprime la imagen como fondo de página
                objPdfDocument.Add(objImagePdf);
             

            }
            catch (Exception ex)
            {
                
                throw ex;
            }


        }

    }
}
