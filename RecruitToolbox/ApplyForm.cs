using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using System.Text.RegularExpressions;
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
            fields["head"].SetValue(StringReplace(FormSettings.Head));
            fields["title"].SetValue(StringReplace(FormSettings.Title));
            fields["name"].SetValue(Applicant.Name);
            fields["sid"].SetValue(Applicant.Sid);
            fields["college"].SetValue(Applicant.College);
            fields["district"].SetValue(Applicant.District);
            fields["tel"].SetValue(Applicant.Tel);
            fields["mail"].SetValue(Applicant.Mail);
            fields["applying"].SetValue(string.Join('\t', Applicant.Applying));
            fields["resume"].SetValue(Applicant.Resume);
            fields["note"].SetValue(StringReplace(FormSettings.Note));

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
            string result =
                originalString
                    .Replace(@"[name]", Applicant.Name)
                    .Replace(@"[sid]", Applicant.Sid)
                    .Replace(@"[college]", Applicant.College)
                    .Replace(@"[district]", Applicant.District)
                    .Replace(@"[tel]", Applicant.Tel)
                    .Replace(@"[mail]", Applicant.Mail)
                    .Replace(@"[resume]", Applicant.Resume)
                    .Replace(@"[applying]", string.Join(", ", Applicant.Applying));

            return result;
        }

    }
}
