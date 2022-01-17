using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Insurance_app.Models
{
    public  class Quote
    {
        public int Age { get; set; }
        public int Hospitals {get; set; }
        public int Cover {get; set; }
        public int Hospital_Excess {get; set; }
        public int Plan {get; set; }
        public int Smoker { get; set;}


        public enum CoverEnum {Low,Medium,High}
        public enum PlanEnum{ Low,Medium,High}

        public IList<String> HospitalsEnum()
        {
            return new List<String>() {"Public Hospitals", "Most Hospitals", "All Hospitals"};
        }
        public IList<int> ExcessFee()
        {
            return new List<int>() {300, 150, 0};
        }
        /*
        quote = new Quote()
        {
            Hospitals = 0,
            Age = 18,
            Cover = 0,
            Hospital_Excess = 150,
            Plan = 0,
            Smoker = 0
        };
        */
    }
}