using System;

namespace RecruitToolbox
{
    class Applicant
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Sid { get; set; }
        public string Tel { get; set; }
        public string Mail { get; set; }
        public string College { get; set; }
        public string District { get; set; }
        public string[] ApplyingArray { get; set; }

        public string Applying
        {
            get => string.Join(';', ApplyingArray);
            set => ApplyingArray = value.Split(new []{';',',','|'}, StringSplitOptions.RemoveEmptyEntries);
        }
        public string Resume { get; set; }
    }
}
