using CDBurner.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace CDBurner.Model
{
    public class Study : ObservableObject //Napraviti model koji odgovara rezultatu API poziva
    {
        private string _patient;
        public string Patient { get => _patient; set { _patient = value; OnPropertyChanged(); } }

        private string _id;
        public string Id { get => _id; set { _id = value; OnPropertyChanged(); } }

        private DateTime _date;
        public DateTime Date { get => _date; set { _date = value; OnPropertyChanged(); } }
    }
}