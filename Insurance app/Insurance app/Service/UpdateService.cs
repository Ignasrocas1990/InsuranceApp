using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Insurance_app.Models;
using Insurance_app.SupportClasses;
using Realms;

namespace Insurance_app.Service
{
    public class UpdateService
    {
        public List<MovData> ToUpdateList;
        public Reward CurrentReward;
        public double NrToMax = 0;
        
        public bool IsStarted { get; set; } = true;

        public UpdateService(Reward reward)
        {
            CurrentReward = reward;
            NrToMax = StaticOptions.StepNeeded-reward.MovData.Count;
            ToUpdateList = new List<MovData>();
        }

        public void SetReward(Reward reward)
        {
            CurrentReward = reward;
            NrToMax = StaticOptions.StepNeeded-reward.MovData.Count;
        }

        public void UpdateNrToMax()
        {
            NrToMax = StaticOptions.StepNeeded-CurrentReward.MovData.Count;

        }
//-------------------------------- Need to be tested ---------------------------------------
        public void UpdateMovData()
        {
            Task t = Task.Run(async () =>
            {
                await Task.Delay(StaticOptions.MovUpdateTimeDelay);
                if (ToUpdateList.Count!=0)
                {
                    if (NrToMax >=ToUpdateList.Count)
                    {
                        await CurrentReward.AddMovData(ToUpdateList);
                    }
                    else
                    {
                        var newL =ToUpdateList.GetRange(0, ToUpdateList.Count);
                        ToUpdateList.RemoveRange(0,ToUpdateList.Count);
                        await CurrentReward.AddMovData(newL);
                    }
                    UpdateNrToMax();

                }
                if (IsStarted)
                {
                    UpdateMovData();
                }
            });
        }
    }
}