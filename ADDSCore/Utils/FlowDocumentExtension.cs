using System;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Xml.Linq;
using System.IO;
using System.IO.Packaging;
using System.Linq;

namespace ADDSCore.Utils
{
    public static class FlowDocumentExtension
    {
       public static void LoadWordML(this FlowDocument doc, string filename)
        {
            XElement wordDoc = null;
            try
            {
                Package package = Package.Open(filename);
                Uri documentUri = new Uri("/word/document.xml", UriKind.Relative);
                PackagePart docPart = package.GetPart(documentUri);
                wordDoc = XElement.Load(new StreamReader(docPart.GetStream()));
            }
            catch(Exception)
            {
                doc.Blocks.Add(new Paragraph(new Run("File not found or not in correct format (Word 2007)")));
                return;
            }

            XNamespace w = "http://schemas.openxmlformats.org/wordprocessingml/2006/main";
            var paragraphs = from p in wordDoc.Descendants(w + "p") select p;

            foreach(var p in paragraphs)
            {
                var style = from s in p.Descendants(w + "pPr") select s;

                var font = (from f in style.Descendants(w + "rFonts")
                            select f.FirstAttribute).FirstOrDefault();
                var size = (from s in style.Descendants(w + "sz")
                            select s.FirstAttribute).FirstOrDefault();

                Paragraph par = new Paragraph();
                Run r = new Run(p.Value);
                if(font != null)
                {
                    FontFamilyConverter converter = new FontFamilyConverter();
                    r.FontFamily = (FontFamily)converter.ConvertFrom(font.Value);
                }
                if(size != null)
                {
                    r.FontSize = double.Parse(size.Value);
                }
                par.Inlines.Add(r);

                doc.Blocks.Add(par);
            }
        }
    }
}
