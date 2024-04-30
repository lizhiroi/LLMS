using Microsoft.VisualStudio.TestTools.UnitTesting;
using LLMS.ViewModel; // Assuming your ViewModel namespace

namespace LLMS.Tests
{
    [TestClass]
    public class LeaseWindowTests
    {
        private LeaseWindowViewModel _viewModel;

        [TestInitialize]
        public void SetUp()
        {
            // Initialize ViewModel
            _viewModel = new LeaseWindowViewModel();
        }

        [TestMethod]
        public void TestLeaseDataRetrieval()
        {
            // Arrange
            int expectedLeaseCount = 1; // Assuming at least one lease is expected in the database

            // Act
            _viewModel.LoadLeaseData();

            // Assert
            Assert.IsNotNull(_viewModel.Leases, "Leases collection is null");
            Assert.AreEqual(expectedLeaseCount, _viewModel.Leases.Count, "Unexpected number of leases retrieved from the database");
        }
    }
}