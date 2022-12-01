using DigitalTwin.Middleware.DataInput.Cache;
using DigitalTwin.Middleware.DataInput.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace DigitalTwin.Middleware.DataInput
{
    public class UserPointCalculator
    {
        private readonly RecordingCache recordingCache;

        public UserPointCalculator(RecordingCache recordingCache)
        {
            this.recordingCache = recordingCache;
        }

        public UserBehaviourInput? CalculateNextStep(HapticOutput hapticOutput, int step)
        {
            var currentUserStep = step;
            var nextUserStep = step + 1;

            Data? currentRecordingData;
            Data? nextRecordingData;
            try
            {
                currentRecordingData = recordingCache.GetData()[currentUserStep];
                nextRecordingData = recordingCache.GetData()[nextUserStep];
            }
            catch (Exception)
            {
                return null;
            }


            var currentXUserInput = currentRecordingData.Pos[0];
            var currentYUserInput = currentRecordingData.Pos[1];
            var currentZUserInput = currentRecordingData.Pos[2];

            var nextXUserInput = nextRecordingData.Pos[0];
            var nextYUserInput = nextRecordingData.Pos[1];
            var nextZUserInput = nextRecordingData.Pos[2];


            var randomNoise = GenerateRandomNoise();

            var newUserXStep = currentXUserInput + hapticOutput.OutputHapticFeedbackX + (nextXUserInput - currentXUserInput) + randomNoise;
            var newUserYStep = currentYUserInput + hapticOutput.OutputHapticFeedbackY + (nextYUserInput - currentYUserInput) + randomNoise;
            var newUserZStep = currentZUserInput + hapticOutput.OutputHapticFeedbackZ + (nextZUserInput - currentZUserInput) + randomNoise;

            return new UserBehaviourInput(
                newUserXStep,
                newUserYStep,
                newUserZStep
                );
        }

        private float GenerateRandomNoise()
        {
            var random = new Random();

            var randomDouble = random.NextDouble(-0.05, 0.05);

            return (float) randomDouble + 0.001F;
        }
    }
}
