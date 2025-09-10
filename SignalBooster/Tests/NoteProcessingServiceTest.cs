using System.Text.Json.Nodes;
using SignalBoosterCLI.Services;

namespace Tests;

public class NoteProcessingServiceTest
{
        private readonly NoteProcessingService _service;

        public NoteProcessingServiceTest()
        {
            _service = new NoteProcessingService();
        }
        
        [Fact]
        public void ExtractOrder_FromFile_ShouldMatchExpectedJson()
        {
            // Arrange
            string filePath = Path.Combine("TestFiles", "physician_note1.txt");
            string note = File.ReadAllText(filePath);

            // Act
            JsonObject result = _service.ExtractOrder(note);

            // Assert
            Assert.Equal("Oxygen Tank", result["device"]?.ToString());
            Assert.Null(result["mask_type"]);
            Assert.Null(result["add_ons"]);
            Assert.Equal("", result["qualifier"]?.ToString());
            Assert.Equal("Dr. Cuddy\r\n", result["ordering_provider"]?.ToString());
            Assert.Equal("2 L", result["liters"]?.ToString());
            Assert.Equal("sleep and exertion", result["usage"]?.ToString());
        }


        [Fact]
        public void ExtractOrder_ShouldDetectCPAPDevice()
        {
            // Arrange
            string note = "Patient needs a CPAP with full face mask and humidifier. AHI > 20. Ordered by Dr. Cameron.";

            // Act
            JsonObject result = _service.ExtractOrder(note);

            // Assert
            Assert.Equal("CPAP", result["device"]?.ToString());
            Assert.Equal("full face", result["mask_type"]?.ToString());
            Assert.Equal("AHI > 20", result["qualifier"]?.ToString());
            Assert.Equal("Dr. Cameron", result["ordering_provider"]?.ToString());

            var addOns = result["add_ons"] as JsonArray;
            Assert.NotNull(addOns);
            Assert.Contains("humidifier", addOns.Select(x => x.ToString()));
        }

        [Fact]
        public void ExtractOrder_ShouldDetectOxygenTankWithUsage()
        {
            // Arrange
            string note = "Patient requires 2.5 L oxygen during sleep and exertion. Ordered by Dr. House.";

            // Act
            JsonObject result = _service.ExtractOrder(note);

            // Assert
            Assert.Equal("Oxygen Tank", result["device"]?.ToString());
            Assert.Equal("2.5 L", result["liters"]?.ToString());
            Assert.Equal("sleep and exertion", result["usage"]?.ToString());
            Assert.Equal("Dr. House", result["ordering_provider"]?.ToString());
        }

        [Fact]
        public void ExtractOrder_ShouldReturnUnknownDeviceForUnrecognizedNote()
        {
            // Arrange
            string note = "Patient needs mobility assistance. Ordered by Dr. Strange.";

            // Act
            JsonObject result = _service.ExtractOrder(note);

            // Assert
            Assert.Equal("Unknown", result["device"]?.ToString());
            Assert.Equal("Dr. Strange", result["ordering_provider"]?.ToString());
        }

}