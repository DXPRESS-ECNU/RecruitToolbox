using System;
using iText.Barcodes;
using iText.Forms;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using System.Collections.Generic;
using System.IO;

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

                var sap = new PdfDocument(new PdfReader(singleApplyForm.GetPage()));
                sap.CopyPagesTo(1, sap.GetNumberOfPages(), pdfDocument);
            }
            
            return pdfDocument;
        }

        public MemoryStream GetPage()
        {
            MemoryStream pdfStream = new MemoryStream();
            PdfWriter writer = new PdfWriter(pdfStream);
            writer.SetCloseStream(false);
            var template = new PdfDocument(new PdfReader(TemplatePdf), writer);
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
            fields["apply"].SetValue(string.Join('\t', Applicant.ApplyingArray));
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
            template.Close();
            pdfStream.Position = 0;
            return pdfStream;
        }
        public string StringReplace(string originalString)
        {
            if (string.IsNullOrEmpty(originalString))
            {
                return String.Empty;
            }
            string result =
                originalString
                    .Replace(@"[name]", Applicant.Name)
                    .Replace(@"[id]", Applicant.Id)
                    .Replace(@"[sid]", Applicant.Sid)
                    .Replace(@"[college]", Applicant.College)
                    .Replace(@"[district]", Applicant.District)
                    .Replace(@"[tel]", Applicant.Tel)
                    .Replace(@"[mail]", Applicant.Mail)
                    .Replace(@"[resume]", Applicant.Resume)
                    .Replace(@"[apply]", string.Join(", ", Applicant.ApplyingArray));

            return result;
        }

    }
}
