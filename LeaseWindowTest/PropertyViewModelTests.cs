using LLMS;
using LLMS.Dto;
using LLMS.Service;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PropertyUnitTest
{
    [TestClass]
    public class PropertyViewModelTests
    {
        [TestMethod]
        public void Constructor_ShouldInitializeCommands()
        {
            // Arrange
            var propertyServiceMock = new Mock<IPropertyService>();
            var imageServiceMock = new Mock<IImageService>();

            // Act
            var viewModel = new PropertyViewModel(propertyServiceMock.Object, imageServiceMock.Object);

            // Assert
            Assert.IsNotNull(viewModel.UploadImageCommand);
            Assert.IsNotNull(viewModel.SavePropertyCommand);
            Assert.IsNotNull(viewModel.DeletePropertyCommand);
        }

        [TestMethod]
        public void SelectedProperty_SetNewValue_ShouldRaisePropertyChanged()
        {
            // Arrange
            var propertyServiceMock = new Mock<IPropertyService>();
            var imageServiceMock = new Mock<IImageService>();
            var viewModel = new PropertyViewModel(propertyServiceMock.Object, imageServiceMock.Object);
            bool propertyChangedRaised = false;

            viewModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(PropertyViewModel.SelectedProperty))
                    propertyChangedRaised = true;
            };

            // Act
            viewModel.SelectedProperty = new PropertyDto();

            // Assert
            Assert.IsTrue(propertyChangedRaised);
        }
        [TestMethod]
        public void UploadImageCommand_CanExecute_ShouldReturnTrueWhenSelectedPropertyIsNotNull()
        {
            // Arrange
            var propertyServiceMock = new Mock<IPropertyService>();
            var imageServiceMock = new Mock<IImageService>();
            var viewModel = new PropertyViewModel(propertyServiceMock.Object, imageServiceMock.Object);
            viewModel.SelectedProperty = new PropertyDto();

            // Act
            bool canExecute = viewModel.UploadImageCommand.CanExecute();

            // Assert
            Assert.IsTrue(canExecute);
        }
        [TestMethod]
        public async Task SavePropertyCommand_Executed_ShouldAddOrUpdateProperty()
        {
            // Arrange
            var propertyServiceMock = new Mock<IPropertyService>();
            var imageServiceMock = new Mock<IImageService>();
            var viewModel = new PropertyViewModel(propertyServiceMock.Object, imageServiceMock.Object);
            viewModel.SelectedProperty = new PropertyDto { Address = "Test Address" };

            propertyServiceMock.Setup(p => p.CreatePropertyAsync(It.IsAny<PropertyDto>()))
                .ReturnsAsync(new PropertyDto { Id = 1, Address = "Test Address" });

            // Act
            viewModel.SavePropertyCommand.Execute();

            // Assert
            propertyServiceMock.Verify(p => p.CreatePropertyAsync(It.IsAny<PropertyDto>()), Times.Once);
            // You can also check if the Properties collection has been updated accordingly
        }
        [TestMethod]
        public async Task DeletePropertyCommand_Executed_ShouldRemoveProperty()
        {
            // Arrange
            var propertyServiceMock = new Mock<IPropertyService>();
            var imageServiceMock = new Mock<IImageService>();
            var viewModel = new PropertyViewModel(propertyServiceMock.Object, imageServiceMock.Object);
            var testProperty = new PropertyDto { Id = 1, Address = "Test Address" };
            viewModel.Properties.Add(testProperty);
            viewModel.SelectedProperty = testProperty;

            propertyServiceMock.Setup(p => p.DeletePropertyAsync(It.IsAny<int>()))
                .ReturnsAsync(true);

            // Act
            viewModel.DeletePropertyCommand.Execute();

            // Assert
            propertyServiceMock.Verify(p => p.DeletePropertyAsync(It.IsAny<int>()), Times.Once);
            Assert.IsFalse(viewModel.Properties.Contains(testProperty));
        }
        [TestMethod]
        public async Task LoadPropertiesAsync_ShouldPopulatePropertiesCollection()
        {
            // Arrange
            var propertyServiceMock = new Mock<IPropertyService>();
            var imageServiceMock = new Mock<IImageService>();
            var propertiesList = new List<PropertyDto>
    {
        new PropertyDto { Id = 1, Address = "Address 1" },
        new PropertyDto { Id = 2, Address = "Address 2" }
    };

            propertyServiceMock.Setup(p => p.GetAllPropertiesAsync())
                .ReturnsAsync(propertiesList);

            var viewModel = new PropertyViewModel(propertyServiceMock.Object, imageServiceMock.Object);

            // Act
            viewModel.LoadPropertiesAsync(); // Assuming LoadPropertiesAsync is public for testing

            // Assert
            Assert.AreEqual(propertiesList.Count, viewModel.Properties.Count - 1); // Exclude the "Add new property..." item
        }
        [TestMethod]
        public void SaveAndDeleteCommand_CanExecute_ShouldReturnFalseWhenNoPropertySelected()
        {
            // Arrange
            var propertyServiceMock = new Mock<IPropertyService>();
            var imageServiceMock = new Mock<IImageService>();
            var viewModel = new PropertyViewModel(propertyServiceMock.Object, imageServiceMock.Object);

            // Act
            bool canSaveExecute = viewModel.SavePropertyCommand.CanExecute();
            bool canDeleteExecute = viewModel.DeletePropertyCommand.CanExecute();

            // Assert
            Assert.IsFalse(canSaveExecute, "SavePropertyCommand can execute without a selected property.");
            Assert.IsFalse(canDeleteExecute, "DeletePropertyCommand can execute without a selected property.");
        }
        [TestMethod]
        public async Task ViewModelInitialization_ShouldLoadProperties()
        {
            // Arrange
            var propertyServiceMock = new Mock<IPropertyService>();
            var expectedProperties = new List<PropertyDto>
    {
        new PropertyDto { Id = 1, Address = "Address 1" },
        new PropertyDto { Id = 2, Address = "Address 2" }
    };
            propertyServiceMock.Setup(p => p.GetAllPropertiesAsync()).ReturnsAsync(expectedProperties);
            var imageServiceMock = new Mock<IImageService>();

            // Act
            var viewModel = new PropertyViewModel(propertyServiceMock.Object, imageServiceMock.Object);

            // Use a small delay to ensure async LoadPropertiesAsync method completes
            await Task.Delay(100);

            // Assert
            Assert.AreEqual(expectedProperties.Count + 1, viewModel.Properties.Count, "Properties collection was not loaded correctly on ViewModel initialization.");
        }
        [TestMethod]
        public async Task SavePropertyCommand_WhenServiceThrowsException_ShouldUpdateStatusMessage()
        {
            // Arrange
            var propertyServiceMock = new Mock<IPropertyService>();
            var imageServiceMock = new Mock<IImageService>();
            propertyServiceMock.Setup(p => p.CreatePropertyAsync(It.IsAny<PropertyDto>())).ThrowsAsync(new Exception("Service error"));

            var viewModel = new PropertyViewModel(propertyServiceMock.Object, imageServiceMock.Object)
            {
                SelectedProperty = new PropertyDto { Address = "Test Address" }
            };

            // Act
            viewModel.SavePropertyCommand.Execute();
            await Task.Delay(100); 

            // Assert
            StringAssert.Contains(viewModel.StatusMessage, "error", "StatusMessage does not contain error information after a failed save operation.");
        }
    }
}
