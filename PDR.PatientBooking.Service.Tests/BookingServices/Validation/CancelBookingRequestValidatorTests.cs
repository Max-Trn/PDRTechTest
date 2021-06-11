using AutoFixture;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using PDR.PatientBooking.Data;
using PDR.PatientBooking.Data.Models;
using PDR.PatientBooking.Service.BookingServices.Validation;
using PDR.PatientBooking.Service.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PDR.PatientBooking.Service.Tests.BookingServices.Validation
{
    public class CancelBookingRequestValidatorTests
    {
        private IFixture _fixture;

        private ICancelBookingRequestValidator _cancelBookingRequestValidator;

        [SetUp]
        public void SetUp()
        {
            // Boilerplate
            _fixture = new Fixture();

            //Prevent fixture from generating from entity circular references 
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior(1));

            // Mock setup

            // Sut instantiation
            _cancelBookingRequestValidator = new CancelBookingRequestValidator();
        }

        [Test]
        public void ValidateRequest_CanceledBooking_ReturnsFailedValidationResult()
        {
            //arrange
            var order = GetValidOrder();
            order.State = (int)BookingState.Canceled;
            //act
            var res = _cancelBookingRequestValidator.ValidateRequest(order);

            //assert
            res.PassedValidation.Should().BeFalse();
            res.Errors.Should().Contain("Specified appointment already canceled");
        }

        [Test]
        public void ValidateRequest_BookingDoesNotExist_ReturnsFailedValidationResult()
        {
            //arrange
            Order order = null;
            //act
            var res = _cancelBookingRequestValidator.ValidateRequest(order);

            //assert
            res.PassedValidation.Should().BeFalse();
            res.Errors.Should().Contain("Specified appointment does not exist");
        }

        [Test]
        public void ValidateRequest_ValidOrder_ReturnsPassedValidationResult()
        {
            //arrange
            var order = GetValidOrder();

            //act
            var res = _cancelBookingRequestValidator.ValidateRequest(order);

            //assert
            res.PassedValidation.Should().BeTrue();
        }

        private Order GetValidOrder()
        {
            var order = _fixture.Build<Order>()
                .With(x => x.State, (int)BookingState.Active)
                .Create();
            return order;
        }
    }
}
