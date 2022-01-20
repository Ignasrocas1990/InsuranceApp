using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Insurance_app.Models
{
    public  class QuoteOptions
    {
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
    }
}