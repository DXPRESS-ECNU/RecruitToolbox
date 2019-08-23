using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using iText.Barcodes;
using iText.Forms;
using iText.Forms.Fields;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Layout;
using iText.Layout.Element;

namespace RecruitToolbox
{
    class ApplyForm
    {
        public static string TemplatePdf { get; set; }
        public Applicant Applicant { get; set; }
        public static FormSettings FormSettings { get; set; }
        public ApplyForm(Applicant applicant)
        {
            Applicant = applicant;
        }

        public static void GeneratePdfForms(string pdfFilename, IEnumerable<Applicant> applicants) => GeneratePdfForms(new PdfDocument(new PdfWriter(pdfFilename)), applicants).Close();

        public static PdfDocument GeneratePdfForms(PdfDocument pdfDocument,IEnumerable<Applicant> applicants)
        {
            foreach (Applicant applicant in applicants)
            {
                ApplyForm singleApplyForm = new ApplyForm(applicant);
                pdfDocument.AddPage(singleApplyForm.GetPage());
            }
            
            return pdfDocument;
        }

        public PdfPage GetPage()
        {
            var template = new PdfDocument(new PdfReader(TemplatePdf));
            var form = PdfAcroForm.GetAcroForm(template, false);
            var fields = form.GetFormFields();
            fields["head"].SetValue(FormSettings.Head);
            fields["title"].SetValue(FormSettings.Title);
            fields["name"].SetValue(Applicant.Name);
            fields["sid"].SetValue(Applicant.Sid);
            fields["college"].SetValue(Applicant.College);
            fields["district"].SetValue(Applicant.District);
            fields["tel"].SetValue(Applicant.Tel);
            fields["mail"].SetValue(Applicant.Mail);
            
            //TODO complete field["applying"]

            fields["resume"].SetValue(Applicant.Resume);
            fields["note"].SetValue(FormSettings.Note);

            if (FormSettings.IsAddBarcode)
            {
                Barcode128 barcode = new Barcode128(template);
                barcode.SetCode(Applicant.Sid);
                Image image = new Image(barcode.CreateFormXObject(template));
                image.SetFixedPosition(430, 790);
                Document document = new Document(template);
                document.Add(image);
            }
            
            form.FlattenFields();
            return template.GetFirstPage();
        }
        public string StringReplace(string originalString)
        {
            Dictionary<string,string> dictionary = StrToDic(originalString);
            dictionary["head"] = FormSettings.Head;
            dictionary["title"] = FormSettings.Title;
            dictionary["name"] = Applicant.Name;
            dictionary["sid"] = Applicant.Sid;
            dictionary["college"] = Applicant.College;
            dictionary["district"] = Applicant.District;
            dictionary["tel"] = Applicant.Tel;
            dictionary["mail"] = Applicant.Mail;

            //TODO add applying

            dictionary["resume"] = Applicant.Resume;
            dictionary["note"] = FormSettings.Note;
            return DicToStr(dictionary);
        }

        private Dictionary<string, string> StrToDic(string originalString)
        {
            string[] temps = originalString.Split('-');
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            foreach (var temp in temps)
            {
                string[] information = temp.Split(':');
                dictionary.Add(information[0], information[1]);
            }

            return dictionary;
        }

        private string DicToStr(Dictionary<string, string> dictionary)
        {
            string result = "";
            foreach (var information in dictionary)
            {
                result += $"{information.Key}:{information.Value}-";
            }

            return result.Substring(result.Length - 1, 1);
        }
    }
}
