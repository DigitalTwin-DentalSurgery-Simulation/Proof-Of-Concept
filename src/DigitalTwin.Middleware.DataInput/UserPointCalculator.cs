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

        public UserBehaviourInput? CalculateNextStep(HapticOutput hapticOutput)
        {
            var currentUserStep = hapticOutput.StepSize;
            var nextUserStep = hapticOutput.StepSize + 1;

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

            var random = new Random();

            var randomMirrorMovementFactorX = (float) random.NextDouble(0.2, 1.8);
            var randomMirrorMovementFactorY = (float)random.NextDouble(0.2, 1.8);
            var randomMirrorMovementFactorZ = (float)random.NextDouble(0.2, 1.8);
            /*
            var randomExtramovementX = 0.0F;
            var randomExtraMovementY = 0.0F;
            var randomExtraMovementZ = 0.0F;
            */
            //var randomDouble = random.NextDouble(1, 2);
            //var negativeOrPositive = -1.0F;

            if (hapticOutput.StepSize % 120 <= 60)
            {
                //negativeOrPositive = 1.0F;
            }


            var x = (hapticOutput.StepSize % 90) * 4;

            var radiantInput = (x * (Math.PI)) / 180;

            var y = Math.Sin(radiantInput) / 12.0F;
            var z = Math.Sin(radiantInput) / 12.0F;

            /*
            if (hapticOutput.StepSize % 60 <= 20)
            {
                // Add a more stepwise increase. like modulus + 0.05 -> +0,1 -> +40,15
                var stepwiseBasedIncrement = (0.10 * (hapticOutput.StepSize % 30));

                randomExtramovementX = (float)stepwiseBasedIncrement * negativeOrPositive;
                randomExtraMovementY = (float)stepwiseBasedIncrement * negativeOrPositive;
                randomExtraMovementZ = (float)stepwiseBasedIncrement * negativeOrPositive;
            }
            */

            Console.WriteLine($"Random: {randomMirrorMovementFactorX} - {randomMirrorMovementFactorY}");

            Console.WriteLine($"Haptic Output: {hapticOutput.OutputHapticFeedbackX} - {hapticOutput.OutputHapticFeedbackY} - {hapticOutput.OutputHapticFeedbackZ}");

            Console.WriteLine($"Sinus Value y: {y} - Sinus Value Z: {z}");

            var newUserXStep = hapticOutput.OutputUserPosXToMiddleware + hapticOutput.OutputHapticFeedbackX + (nextXUserInput - currentXUserInput)* randomMirrorMovementFactorX;
            var newUserYStep = hapticOutput.OutputUserPosYToMiddleware + hapticOutput.OutputHapticFeedbackY + (nextYUserInput - currentYUserInput)* randomMirrorMovementFactorY + (float)y;
            var newUserZStep = hapticOutput.OutputUserPosZToMiddleware + hapticOutput.OutputHapticFeedbackZ + (nextZUserInput - currentZUserInput)* randomMirrorMovementFactorZ + (float)z;

            if (newUserXStep == 0.0F)
                Console.WriteLine(
                    $"NewUserStepX = {newUserXStep} \n" +
                    $"NewUserStepY = {newUserYStep} \n" +
                    $"NewUserStepZ = {newUserZStep} \n"
                    );

            return new UserBehaviourInput(
                newUserXStep,
                newUserYStep,
                newUserZStep,
                nextUserStep
                );
        }

        private float GenerateRandomNoise()
        {
            var random = new Random();

            var randomDouble = random.NextDouble(-0.03, 0.03);

            return (float)randomDouble;
        }
    }
}
