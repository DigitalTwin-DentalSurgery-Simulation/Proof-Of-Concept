using DigitalTwin.Middleware.DataInput.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalTwin.Middleware.DataInput
{
    public class UserPointCalculator
    {
        public static UserBehaviourInput CalculateNextStep(HapticOutput hapticOutput, int step)
        {
            return new UserBehaviourInput(
                0.0F,
                0.0F,
                0.0F,
                step
                );
        }
    }
}
