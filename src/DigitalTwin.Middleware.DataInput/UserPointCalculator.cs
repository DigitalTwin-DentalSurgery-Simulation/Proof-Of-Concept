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

            var randomDouble = random.NextDouble(1, 2);
            var negativeOrPositive = -1.5F;

            if(randomDouble >= 1.5)
            {
                negativeOrPositive = 1.5F;
            }

            if(hapticOutput.StepSize % 20 <= 4)
            {
                // Add a more stepwise increase. like modulus + 0.05 -> +0,1 -> +40,15
                randomMirrorMovementFactorX *= negativeOrPositive;
                randomMirrorMovementFactorY *= negativeOrPositive;
                randomMirrorMovementFactorZ *= negativeOrPositive;
            }

            Console.WriteLine($"Random: {randomMirrorMovementFactorX} - {randomMirrorMovementFactorY}");

            Console.WriteLine($"Haptic Output: {hapticOutput.OutputHapticFeedbackX} - {hapticOutput.OutputHapticFeedbackY} - {hapticOutput.OutputHapticFeedbackZ}");

            var newUserXStep = hapticOutput.OutputUserPosXToMiddleware + hapticOutput.OutputHapticFeedbackX + (nextXUserInput- currentXUserInput)* randomMirrorMovementFactorX;
            var newUserYStep = hapticOutput.OutputUserPosYToMiddleware + hapticOutput.OutputHapticFeedbackY + (nextYUserInput- currentYUserInput)* randomMirrorMovementFactorY;
            var newUserZStep = hapticOutput.OutputUserPosZToMiddleware + hapticOutput.OutputHapticFeedbackZ + (nextZUserInput - currentZUserInput)* randomMirrorMovementFactorZ;

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
