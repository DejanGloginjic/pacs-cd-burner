using CDBurner.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace CDBurner.Model
{
    public class StudyModel : ObservableObject
    {
        private string _id;
        public string Id { get => _id; set { _id = value; OnPropertyChanged(); } }

        private string _patient;
        public string Patient { get => _patient; set { _patient = value; OnPropertyChanged(); } }

        private string _patientId;
        public string PatientId { get => _patientId; set { _patientId = value; OnPropertyChanged(); } }

        private string _patientSex;
        public string PatientSex { get => _patientSex; set { _patientSex = value; OnPropertyChanged(); }  }

        private string _physician;
        public string Physician { get => _physician; set { _physician = value; OnPropertyChanged(); } }

        private string _modality;
        public string Modality { get => _modality; set { _modality = value; OnPropertyChanged(); } }

        private DateTime _dateFrom;
        public DateTime DateFrom { get => _dateFrom; set { _dateFrom = value; OnPropertyChanged(); } }

        private DateTime _dateTo;
        public DateTime DateTo { get => _dateTo; set { _dateTo = value; OnPropertyChanged(); } }

        private string _url;
        public string Url { get => _url; set { _url = value; OnPropertyChanged(); } }

        private string _description;
        public string Description { get => _description; set { _description = value; OnPropertyChanged(); } }
    }
}