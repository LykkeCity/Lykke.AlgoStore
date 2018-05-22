using System;
using Lykke.AlgoStore.Core.Domain.Validation;
using Lykke.AlgoStore.Core.Validation;
using Lykke.AlgoStore.Services;
using NUnit.Framework;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Lykke.AlgoStore.Tests.Unit
{
    [TestFixture]
    public class CodeValidationTests
    {
        [Test]
        public void SyntaxValidation_Fails_WhenNoClassInheritsBaseAlgo()
        {
            var code = "class A {} class B : C {}";

            var result = When_Code_IsSyntaxValidated(code);

            Then_Result_MustFailAndContainMessages(result);
            Then_Result_MustContainMessage(result, "AS0001");
        }

        [Test]
        public void SyntaxValidation_Fails_WhenMultipleClassesInheritBaseAlgo()
        {
            var code = "class A : BaseAlgo {} class B : BaseAlgo {}";

            var result = When_Code_IsSyntaxValidated(code);

            Then_Result_MustFailAndContainMessages(result);
            Then_Result_MustContainMessage(result, "AS0002");
        }

        [Test]
        public void SyntaxValidation_Fails_WhenAlgoNotSealed()
        {
            var code = "class A : BaseAlgo {}";

            var result = When_Code_IsSyntaxValidated(code);

            Then_Result_MustFailAndContainMessages(result);
            Then_Result_MustContainMessage(result, "AS0003");
        }

        [Test]
        public void SyntaxValidation_Fails_WhenTypeNamedBaseAlgo()
        {
            var code = "interface BaseAlgo {} enum BaseAlgo {} class BaseAlgo {} struct BaseAlgo {}";

            var result = When_Code_IsSyntaxValidated(code);

            Then_Result_MustFailAndContainMessages(result);
            Then_Result_MustContainMessageNTimes(result, "AS0004", 4);
        }

        [Test]
        public void SyntaxValidation_Fails_WhenEventsNotImplemented()
        {
            var code = "class A : BaseAlgo {void A() {} void OnCandleReceived() {}}";

            var result = When_Code_IsSyntaxValidated(code);

            Then_Result_MustFailAndContainMessages(result);
            Then_Result_MustContainMessage(result, "AS0005");
        }

        [Test]
        public void SyntaxValidation_Succeeds_WhenAlgoProperlyImplemented()
        {
            var code = @"using Lykke.AlgoStore.CSharp.AlgoTemplate.Abstractions.Core.Domain; 
                         sealed class A : BaseAlgo 
                         { 
                            public override void OnCandleReceived(ICandleContext context) {} 
                         }";

            var result = When_Code_IsSyntaxValidated(code);

            Then_Result_MustSucceedAndContainNoMessages(result);
        }

        [Test, Explicit]
        public void ExtractMetadata_Succeeds_WhenAlgoProperlyImplemented()
        {
            var base64Content =
                "dXNpbmcgU3lzdGVtOw0KdXNpbmcgTHlra2UuQWxnb1N0b3JlLkNTaGFycC5BbGdvVGVtcGxhdGUuQWJzdHJhY3Rpb25zLkNvcmUuRG9tYWluOw0KdXNpbmcgTHlra2UuQWxnb1N0b3JlLkNTaGFycC5BbGdvVGVtcGxhdGUuQWJzdHJhY3Rpb25zLkZ1bmN0aW9ucy5TTUE7DQoNCm5hbWVzcGFjZSBMeWtrZS5BbGdvU3RvcmUuQ1NoYXJwLkFsZ28uSW1wbGVtZW50aW9uDQp7DQogICAgLy8vIDxzdW1tYXJ5Pg0KICAgIC8vLyBSRU1BUks6IEp1c3QgYSBkdW1teSBhbGdvIGltcGxlbWVudGF0aW9uIGZvciBmdXR1cmUgcmVmZXJlbmNlLg0KICAgIC8vLyBXZSBjYW4gYW5kIHdpbGwgcmVtb3ZlIHRoaXMgd2hlbiBmaXJzdCBhbGdvIGlzIGltcGxlbWVudGVkIDopDQogICAgLy8vIDwvc3VtbWFyeT4NCiAgICBzZWFsZWQgY2xhc3MgRHVtbXlBbGdvIDogQmFzZUFsZ28NCiAgICB7DQogICAgICAgIHB1YmxpYyBTbWFGdW5jdGlvbiBfc2hvcnRTbWE7DQogICAgICAgIHB1YmxpYyBTbWFGdW5jdGlvbiBfbG9uZ1NtYTsNCg0KICAgICAgICBwdWJsaWMgb3ZlcnJpZGUgdm9pZCBPblN0YXJ0VXAoSUZ1bmN0aW9uUHJvdmlkZXIgZnVuY3Rpb25zKQ0KICAgICAgICB7DQoNCiAgICAgICAgICAgIF9zaG9ydFNtYSA9IGZ1bmN0aW9ucy5HZXRGdW5jdGlvbjxTbWFGdW5jdGlvbj4oIlNNQV9TaG9ydCIpOw0KICAgICAgICAgICAgX2xvbmdTbWEgPSBmdW5jdGlvbnMuR2V0RnVuY3Rpb248U21hRnVuY3Rpb24+KCJTTUFfTG9uZyIpOw0KICAgICAgICB9DQoNCiAgICAgICAgcHVibGljIG92ZXJyaWRlIHZvaWQgT25RdW90ZVJlY2VpdmVkKElRdW90ZUNvbnRleHQgY29udGV4dCkNCiAgICAgICAgew0KICAgICAgICAgICAgY29udGV4dC5BY3Rpb25zLkxvZygkIlZvbHVtZSB2YWx1ZToge1ZvbHVtZX0iKTsNCg0KICAgICAgICAgICAgdmFyIHF1b3RlID0gY29udGV4dC5EYXRhLlF1b3RlOw0KICAgICAgICAgICAgY29udGV4dC5BY3Rpb25zLkxvZygkIlJlY2VpdmluZyBxdW90ZSBhdCB7RGF0ZVRpbWUuVXRjTm93fSAiICsNCiAgICAgICAgICAgICAgICAkInt7cXVvdGUuUHJpY2U6IHtxdW90ZS5QcmljZX19fSwge3txdW90ZS5UaW1lc3RhbXA6IHtxdW90ZS5UaW1lc3RhbXB9fX0sICIgKw0KICAgICAgICAgICAgICAgICQie3txdW90ZS5Jc0J1eToge3F1b3RlLklzQnV5fX19LCB7e3F1b3RlLklzT25saW5lOiB7cXVvdGUuSXNPbmxpbmV9fX0iKTsNCg0KICAgICAgICAgICAgdmFyIHNtYVNob3J0ID0gX3Nob3J0U21hLlZhbHVlOw0KICAgICAgICAgICAgdmFyIHNtYUxvbmcgPSBfbG9uZ1NtYS5WYWx1ZTsNCiAgICAgICAgICAgIGNvbnRleHQuQWN0aW9ucy5Mb2coJCJGdW5jdGlvbiB2YWx1ZXMgYXJlOiBTTUFfU2hvcnQ6IHtzbWFTaG9ydH0sIFNNQV9Mb25nOiB7c21hTG9uZ30iKTsNCg0KICAgICAgICAgICAgLy9pZiAocXVvdGUuUHJpY2UgPCAxMDAwMCkNCiAgICAgICAgICAgIC8vew0KICAgICAgICAgICAgY29udGV4dC5BY3Rpb25zLkJ1eShWb2x1bWUpOw0KICAgICAgICAgICAgLy99DQoNCiAgICAgICAgICAgIC8vaWYgKHF1b3RlLlByaWNlID4gNzAwMCkNCiAgICAgICAgICAgIC8vew0KICAgICAgICAgICAgLy8gICAgY29udGV4dC5BY3Rpb25zLlNlbGwoVm9sdW1lKTsNCiAgICAgICAgICAgIC8vfQ0KICAgICAgICB9DQoNCg0KICAgICAgICBwdWJsaWMgb3ZlcnJpZGUgdm9pZCBPbkNhbmRsZVJlY2VpdmVkKElDYW5kbGVDb250ZXh0IGNvbnRleHQpDQogICAgICAgIHsNCiAgICAgICAgICAgIGNvbnRleHQuQWN0aW9ucy5Mb2coJCJWb2x1bWUgdmFsdWU6IHtWb2x1bWV9Iik7DQoNCiAgICAgICAgICAgIHZhciBjYW5kbGUgPSBjb250ZXh0LkRhdGEuQ2FuZGxlOw0KICAgICAgICAgICAgY29udGV4dC5BY3Rpb25zLkxvZygkIlJlY2VpdmluZyBjYW5kbGUgYXQge2NhbmRsZS5EYXRlVGltZX0gY2FuZGxlIGNsb3NlIFByaWNlIHtjYW5kbGUuQ2xvc2V9Iik7DQoNCiAgICAgICAgICAgIHZhciBzbWFTaG9ydCA9IF9zaG9ydFNtYS5WYWx1ZTsNCiAgICAgICAgICAgIHZhciBzbWFMb25nID0gX2xvbmdTbWEuVmFsdWU7DQogICAgICAgICAgICBjb250ZXh0LkFjdGlvbnMuTG9nKCQiRnVuY3Rpb24gdmFsdWVzIGFyZTogU01BX1Nob3J0OiB7c21hU2hvcnR9LCBTTUFfTG9uZzoge3NtYUxvbmd9Iik7DQoNCiAgICAgICAgICAgIC8vaWYgKHF1b3RlLlByaWNlIDwgMTAwMDApDQogICAgICAgICAgICAvL3sNCiAgICAgICAgICAgIC8vY29udGV4dC5BY3Rpb25zLkJ1eShjb250ZXh0LkRhdGEuQ2FuZGxlLCBWb2x1bWUpOw0KICAgICAgICAgICAgLy99DQoNCiAgICAgICAgICAgIC8vaWYgKHF1b3RlLlByaWNlID4gNzAwMCkNCiAgICAgICAgICAgIC8vew0KICAgICAgICAgICAgY29udGV4dC5BY3Rpb25zLlNlbGwoY29udGV4dC5EYXRhLkNhbmRsZSwgVm9sdW1lKTsNCiAgICAgICAgICAgIC8vfQ0KICAgICAgICB9DQogICAgfQ0KfQ==";
            var code = Encoding.UTF8.GetString(Convert.FromBase64String(base64Content));
            var session = GetCSharpCodeBuildSession(code);

            var validationResult = session.Validate().Result;

            var metadata = session.ExtractMetadata().Result;
            var result = JsonConvert.SerializeObject(metadata);

            var base64Result = Convert.ToBase64String(Encoding.UTF8.GetBytes(result));

            Assert.AreEqual("eyJQYXJhbWV0ZXJzIjpbeyJLZXkiOiJBc3NldFBhaXIiLCJWYWx1ZSI6bnVsbCwiVHlwZSI6IlN5c3RlbS5TdHJpbmciLCJQcmVkZWZpbmVkVmFsdWVzIjpudWxsfSx7IktleSI6IkNhbmRsZUludGVydmFsIiwiVmFsdWUiOm51bGwsIlR5cGUiOiJMeWtrZS5BbGdvU3RvcmUuQ1NoYXJwLkFsZ29UZW1wbGF0ZS5Nb2RlbHMuRW51bWVyYXRvcnMuQ2FuZGxlVGltZUludGVydmFsIiwiUHJlZGVmaW5lZFZhbHVlcyI6W3siS2V5IjoiMCIsIlZhbHVlIjoiVW5zcGVjaWZpZWQifSx7IktleSI6IjEiLCJWYWx1ZSI6IlNlYyJ9LHsiS2V5IjoiNjAiLCJWYWx1ZSI6Ik1pbnV0ZSJ9LHsiS2V5IjoiMzAwIiwiVmFsdWUiOiJNaW41In0seyJLZXkiOiI5MDAiLCJWYWx1ZSI6Ik1pbjE1In0seyJLZXkiOiIxODAwIiwiVmFsdWUiOiJNaW4zMCJ9LHsiS2V5IjoiMzYwMCIsIlZhbHVlIjoiSG91ciJ9LHsiS2V5IjoiMTQ0MDAiLCJWYWx1ZSI6IkhvdXI0In0seyJLZXkiOiIyMTYwMCIsIlZhbHVlIjoiSG91cjYifSx7IktleSI6IjQzMjAwIiwiVmFsdWUiOiJIb3VyMTIifSx7IktleSI6Ijg2NDAwIiwiVmFsdWUiOiJEYXkifSx7IktleSI6IjYwNDgwMCIsIlZhbHVlIjoiV2VlayJ9LHsiS2V5IjoiMzAwMDAwMCIsIlZhbHVlIjoiTW9udGgifV19LHsiS2V5IjoiU3RhcnRGcm9tIiwiVmFsdWUiOm51bGwsIlR5cGUiOiJTeXN0ZW0uRGF0ZVRpbWUiLCJQcmVkZWZpbmVkVmFsdWVzIjpudWxsfSx7IktleSI6IkVuZE9uIiwiVmFsdWUiOm51bGwsIlR5cGUiOiJTeXN0ZW0uRGF0ZVRpbWUiLCJQcmVkZWZpbmVkVmFsdWVzIjpudWxsfSx7IktleSI6IlZvbHVtZSIsIlZhbHVlIjpudWxsLCJUeXBlIjoiU3lzdGVtLkRvdWJsZSIsIlByZWRlZmluZWRWYWx1ZXMiOm51bGx9LHsiS2V5IjoiVHJhZGVkQXNzZXQiLCJWYWx1ZSI6bnVsbCwiVHlwZSI6IlN5c3RlbS5TdHJpbmciLCJQcmVkZWZpbmVkVmFsdWVzIjpudWxsfV0sIkZ1bmN0aW9ucyI6W3siVHlwZSI6Ikx5a2tlLkFsZ29TdG9yZS5DU2hhcnAuQWxnb1RlbXBsYXRlLkFic3RyYWN0aW9ucy5GdW5jdGlvbnMuU01BLlNtYUZ1bmN0aW9uIiwiSWQiOiJfc2hvcnRTbWEiLCJGdW5jdGlvblBhcmFtZXRlclR5cGUiOiJMeWtrZS5BbGdvU3RvcmUuQ1NoYXJwLkFsZ29UZW1wbGF0ZS5BYnN0cmFjdGlvbnMuQ29yZS5GdW5jdGlvbnMuRnVuY3Rpb25QYXJhbXNCYXNlIiwiUGFyYW1ldGVycyI6W3siS2V5IjoiQXNzZXRQYWlyIiwiVmFsdWUiOm51bGwsIlR5cGUiOiJTeXN0ZW0uU3RyaW5nIiwiUHJlZGVmaW5lZFZhbHVlcyI6bnVsbH0seyJLZXkiOiJDYW5kbGVPcGVyYXRpb25Nb2RlIiwiVmFsdWUiOm51bGwsIlR5cGUiOiJMeWtrZS5BbGdvU3RvcmUuQ1NoYXJwLkFsZ29UZW1wbGF0ZS5BYnN0cmFjdGlvbnMuQ29yZS5GdW5jdGlvbnMuRnVuY3Rpb25QYXJhbXNCYXNlK0NhbmRsZVZhbHVlIiwiUHJlZGVmaW5lZFZhbHVlcyI6W3siS2V5IjoiMCIsIlZhbHVlIjoiT1BFTiJ9LHsiS2V5IjoiMSIsIlZhbHVlIjoiQ0xPU0UifSx7IktleSI6IjIiLCJWYWx1ZSI6IkxPVyJ9LHsiS2V5IjoiMyIsIlZhbHVlIjoiSElHSCJ9XX0seyJLZXkiOiJGdW5jdGlvbkluc3RhbmNlSWRlbnRpZmllciIsIlZhbHVlIjpudWxsLCJUeXBlIjoiU3lzdGVtLlN0cmluZyIsIlByZWRlZmluZWRWYWx1ZXMiOm51bGx9LHsiS2V5IjoiU3RhcnRpbmdEYXRlIiwiVmFsdWUiOm51bGwsIlR5cGUiOiJTeXN0ZW0uRGF0ZVRpbWUiLCJQcmVkZWZpbmVkVmFsdWVzIjpudWxsfSx7IktleSI6IkVuZGluZ0RhdGUiLCJWYWx1ZSI6bnVsbCwiVHlwZSI6IlN5c3RlbS5EYXRlVGltZSIsIlByZWRlZmluZWRWYWx1ZXMiOm51bGx9LHsiS2V5IjoiQ2FuZGxlVGltZUludGVydmFsIiwiVmFsdWUiOm51bGwsIlR5cGUiOiJMeWtrZS5BbGdvU3RvcmUuQ1NoYXJwLkFsZ29UZW1wbGF0ZS5Nb2RlbHMuRW51bWVyYXRvcnMuQ2FuZGxlVGltZUludGVydmFsIiwiUHJlZGVmaW5lZFZhbHVlcyI6W3siS2V5IjoiMCIsIlZhbHVlIjoiVW5zcGVjaWZpZWQifSx7IktleSI6IjEiLCJWYWx1ZSI6IlNlYyJ9LHsiS2V5IjoiNjAiLCJWYWx1ZSI6Ik1pbnV0ZSJ9LHsiS2V5IjoiMzAwIiwiVmFsdWUiOiJNaW41In0seyJLZXkiOiI5MDAiLCJWYWx1ZSI6Ik1pbjE1In0seyJLZXkiOiIxODAwIiwiVmFsdWUiOiJNaW4zMCJ9LHsiS2V5IjoiMzYwMCIsIlZhbHVlIjoiSG91ciJ9LHsiS2V5IjoiMTQ0MDAiLCJWYWx1ZSI6IkhvdXI0In0seyJLZXkiOiIyMTYwMCIsIlZhbHVlIjoiSG91cjYifSx7IktleSI6IjQzMjAwIiwiVmFsdWUiOiJIb3VyMTIifSx7IktleSI6Ijg2NDAwIiwiVmFsdWUiOiJEYXkifSx7IktleSI6IjYwNDgwMCIsIlZhbHVlIjoiV2VlayJ9LHsiS2V5IjoiMzAwMDAwMCIsIlZhbHVlIjoiTW9udGgifV19XX0seyJUeXBlIjoiTHlra2UuQWxnb1N0b3JlLkNTaGFycC5BbGdvVGVtcGxhdGUuQWJzdHJhY3Rpb25zLkZ1bmN0aW9ucy5TTUEuU21hRnVuY3Rpb24iLCJJZCI6Il9sb25nU21hIiwiRnVuY3Rpb25QYXJhbWV0ZXJUeXBlIjoiTHlra2UuQWxnb1N0b3JlLkNTaGFycC5BbGdvVGVtcGxhdGUuQWJzdHJhY3Rpb25zLkNvcmUuRnVuY3Rpb25zLkZ1bmN0aW9uUGFyYW1zQmFzZSIsIlBhcmFtZXRlcnMiOlt7IktleSI6IkFzc2V0UGFpciIsIlZhbHVlIjpudWxsLCJUeXBlIjoiU3lzdGVtLlN0cmluZyIsIlByZWRlZmluZWRWYWx1ZXMiOm51bGx9LHsiS2V5IjoiQ2FuZGxlT3BlcmF0aW9uTW9kZSIsIlZhbHVlIjpudWxsLCJUeXBlIjoiTHlra2UuQWxnb1N0b3JlLkNTaGFycC5BbGdvVGVtcGxhdGUuQWJzdHJhY3Rpb25zLkNvcmUuRnVuY3Rpb25zLkZ1bmN0aW9uUGFyYW1zQmFzZStDYW5kbGVWYWx1ZSIsIlByZWRlZmluZWRWYWx1ZXMiOlt7IktleSI6IjAiLCJWYWx1ZSI6Ik9QRU4ifSx7IktleSI6IjEiLCJWYWx1ZSI6IkNMT1NFIn0seyJLZXkiOiIyIiwiVmFsdWUiOiJMT1cifSx7IktleSI6IjMiLCJWYWx1ZSI6IkhJR0gifV19LHsiS2V5IjoiRnVuY3Rpb25JbnN0YW5jZUlkZW50aWZpZXIiLCJWYWx1ZSI6bnVsbCwiVHlwZSI6IlN5c3RlbS5TdHJpbmciLCJQcmVkZWZpbmVkVmFsdWVzIjpudWxsfSx7IktleSI6IlN0YXJ0aW5nRGF0ZSIsIlZhbHVlIjpudWxsLCJUeXBlIjoiU3lzdGVtLkRhdGVUaW1lIiwiUHJlZGVmaW5lZFZhbHVlcyI6bnVsbH0seyJLZXkiOiJFbmRpbmdEYXRlIiwiVmFsdWUiOm51bGwsIlR5cGUiOiJTeXN0ZW0uRGF0ZVRpbWUiLCJQcmVkZWZpbmVkVmFsdWVzIjpudWxsfSx7IktleSI6IkNhbmRsZVRpbWVJbnRlcnZhbCIsIlZhbHVlIjpudWxsLCJUeXBlIjoiTHlra2UuQWxnb1N0b3JlLkNTaGFycC5BbGdvVGVtcGxhdGUuTW9kZWxzLkVudW1lcmF0b3JzLkNhbmRsZVRpbWVJbnRlcnZhbCIsIlByZWRlZmluZWRWYWx1ZXMiOlt7IktleSI6IjAiLCJWYWx1ZSI6IlVuc3BlY2lmaWVkIn0seyJLZXkiOiIxIiwiVmFsdWUiOiJTZWMifSx7IktleSI6IjYwIiwiVmFsdWUiOiJNaW51dGUifSx7IktleSI6IjMwMCIsIlZhbHVlIjoiTWluNSJ9LHsiS2V5IjoiOTAwIiwiVmFsdWUiOiJNaW4xNSJ9LHsiS2V5IjoiMTgwMCIsIlZhbHVlIjoiTWluMzAifSx7IktleSI6IjM2MDAiLCJWYWx1ZSI6IkhvdXIifSx7IktleSI6IjE0NDAwIiwiVmFsdWUiOiJIb3VyNCJ9LHsiS2V5IjoiMjE2MDAiLCJWYWx1ZSI6IkhvdXI2In0seyJLZXkiOiI0MzIwMCIsIlZhbHVlIjoiSG91cjEyIn0seyJLZXkiOiI4NjQwMCIsIlZhbHVlIjoiRGF5In0seyJLZXkiOiI2MDQ4MDAiLCJWYWx1ZSI6IldlZWsifSx7IktleSI6IjMwMDAwMDAiLCJWYWx1ZSI6Ik1vbnRoIn1dfV19XX0=", base64Result);
        }

        private ICodeValidationSession GetCSharpCodeBuildSession(string code)
        {
            var codeValidationService = new CodeValidationService();
            var session = codeValidationService.StartSession(code);

            return session;
        }

        private ValidationResult When_Code_IsSyntaxValidated(string code)
        {
            var codeValidationService = new CodeValidationService();
            var session = codeValidationService.StartSession(code);

            return session.Validate().Result;
        }

        private void Then_Result_MustContainMessage(ValidationResult result, string messageId)
        {
            Assert.IsTrue(result.Messages.Any(m => m.Id == messageId));
        }

        private void Then_Result_MustFailAndContainMessages(ValidationResult result)
        {
            Assert.AreEqual(false, result.IsSuccessful);
            Assert.IsTrue(result.Messages.Count > 0);
        }

        private void Then_Result_MustSucceedAndContainNoMessages(ValidationResult result)
        {
            Assert.AreEqual(true, result.IsSuccessful);
            Assert.IsTrue(result.Messages.Count == 0);
        }

        private void Then_Result_MustContainMessageNTimes(ValidationResult result, string messageId, int count)
        {
            Assert.AreEqual(count, result.Messages.Where(m => m.Id == messageId).Count());
        }
    }
}
