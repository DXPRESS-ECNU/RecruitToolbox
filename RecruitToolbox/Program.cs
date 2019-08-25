using CsvHelper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace RecruitToolbox
{
    class Program
    {
        static void Main(string[] args)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            while (true)
            {
                Console.Write("1. Generate Applying Form PDF\n2. Output CSV Template\n0. Exit\nChoose Function:  ");
                string func = Console.ReadLine();

                switch (func)
                {
                    case "0":
                        return;
                    case "1":
                        GenerateForm();
                        break;
                    case "2":
                        CsvTemplate();
                        break;
                }
            }

        }

        static void CsvTemplate()
        {
            Applicant applicant1 = new Applicant
            {
                Sid = "10000000000",
                ApplyingArray = new[] {"新闻部", "新媒体部"},
                College = "传播学院",
                District = "闵行校区",
                Mail = "hello@163.com",
                Name = "张三",
                Resume = "我是张三",
                Tel = "18911111111"
            };
            using (var writer = new StreamWriter(new FileStream("applicants.csv", FileMode.CreateNew), Encoding.GetEncoding("GB2312")))
            {
                using (var csv = new CsvWriter(writer))
                {
                    csv.WriteRecords(new []{applicant1});
                }
            }
        }

        static void GenerateForm()
        {
            GetCsv:
            Console.Write("CSV File: ");
            string applicantCsv = Console.ReadLine();
            if (string.IsNullOrEmpty(applicantCsv) || !File.Exists(applicantCsv))
            {
                goto GetCsv;
            }

            List<Applicant> applicants;
            using (var reader = new StreamReader(new FileStream(applicantCsv, FileMode.Open), Encoding.GetEncoding("GB2312")))
            using (var csv = new CsvReader(reader))
            {
                applicants = csv.GetRecords<Applicant>().ToList();
            }

            LoadTemplate:
            Console.Write("Template Form File[formtemplate.pdf]: ");
            string formTemplate = Console.ReadLine();
            if (string.IsNullOrEmpty(formTemplate))
            {
                formTemplate = "formtemplate.pdf";
            }
            if (!File.Exists(formTemplate))
            {
                goto LoadTemplate;
            }
            ApplyForm.TemplatePdf = formTemplate;

            LoadFormConfig:
            Console.Write("Form Config File[formsettings.json]: ");
            string formSettings = Console.ReadLine();
            if (string.IsNullOrEmpty(formSettings))
            {
                formSettings = "formsettings.json";
            }
            if (!File.Exists(formSettings))
            {
                goto LoadFormConfig;
            }

            string settingJson = File.ReadAllText(formSettings);
            FormSettings fs = JsonConvert.DeserializeObject<FormSettings>(settingJson);
            ApplyForm.FormSettings = fs;

            SavingPath:
            Console.Write("PDF Saving Path: ");
            string savingPath = Console.ReadLine();
            if (string.IsNullOrEmpty(savingPath))
            {
                goto SavingPath;
            }

            ApplyForm.GeneratePdfForms(savingPath, applicants);
        }
    }
}
