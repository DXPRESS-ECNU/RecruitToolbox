using CsvHelper;
using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace RecruitToolbox
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.Write("1. Generate Applying Form PDF\n0. Exit\nChoose Function:  ");
                string func = Console.ReadLine();

                switch (func)
                {
                    case "0":
                        return;
                    case "1":
                        GenerateForm();
                        break;
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

            IEnumerable<Applicant> applicants;
            using (var reader = new StreamReader(applicantCsv))
            using (var csv = new CsvReader(reader))
            {
                applicants = csv.GetRecords<Applicant>();
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
