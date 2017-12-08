﻿namespace BullOak.Repositories.Test.Unit.StateEmit
{
    using System;
    using BullOak.Repositories.StateEmit;
    using FluentAssertions;
    using Xunit;

    public class EmittedTypeFactoryTests
    {
        public interface MyInterface
        {
            int MyValue { get; set; }
        }

        public class MyInterfaceImplementation : MyInterface
        {
            public int MyValue { get; set; }
        }

        public class MyOtherInterfaceImplementation : MyInterface
        {
            public int MyValue { get; set; }

            public MyOtherInterfaceImplementation(int i) => MyValue = i;
        }

        private EmittedTypeFactory sut => new EmittedTypeFactory();

        [Fact]
        public void GetState_OfInterface_ShouldSucceed()
        {
            //Arrange
            var typeOfInterface = typeof(MyInterface);

            //Act
            var instance = sut.GetState(typeOfInterface);

            //
            instance.Should().NotBeNull();
        }

        [Fact]
        public void GetState_OfClassWithDefaultCtor_ShouldSucceed()
        {
            //Arrange
            var typeOfClass = typeof(MyInterfaceImplementation);

            //Act
            var instance = sut.GetState(typeOfClass);

            //
            instance.Should().NotBeNull();
        }

        [Fact]
        public void GetState_OfClassWithDefaultCtor_ShouldReturnInstanceOfClassType()
        {
            //Note: This tests that factory returns instance of actual class and does not use emitter.

            //Arrange
            var typeOfClass = typeof(MyInterfaceImplementation);

            //Act
            var instance = sut.GetState(typeOfClass);

            //
            (instance.GetType() == typeOfClass).Should().BeTrue();
        }

        [Fact]
        public void GetState_OfClassWithoutDefaultCtor_ShouldThrowArgumentException()
        {
            //Arrange
            var typeOfClass = typeof(MyOtherInterfaceImplementation);

            //Act
            var exception = Record.Exception(() => sut.GetState(typeOfClass));

            //
            exception.Should().NotBeNull();
            exception.Should().BeOfType<ArgumentException>();
            (exception as ArgumentException).ParamName.Should().Be("type");
            (exception as ArgumentException).Message.Should().StartWith("Requested type has to have a default ctor");
        }

        [Fact]
        public void GetState_OfInterface_SucceedsAndReturnsInstanceOfTypeThatImplementsInterface()
        {
            //Arrange
            var typeOfInterface = typeof(MyInterface);

            //Act
            var state = sut.GetState(typeOfInterface);

            //Assert
            state.Should().NotBeNull();
            state.Should().BeAssignableTo<MyInterface>();
        }

        [Fact]
        public void GetState_OfInterface_StateAlsoImplementsICanSwitchBackAndToReadOnly()
        {
            //Arrange
            var typeOfInterface = typeof(MyInterface);

            //Act
            var state = sut.GetState(typeOfInterface);

            //Assert
            state.Should().BeAssignableTo<ICanSwitchBackAndToReadOnly>();
        }

        [Fact]
        public void GetState_OfInterface_StateIsImutatableByDefault()
        {
            //Arrange
            var typeOfInterface = typeof(MyInterface);
            var state = sut.GetState(typeOfInterface) as MyInterface;

            //Act
            var exception = Record.Exception(() => state.MyValue = 5);

            //Assert
            exception.Should().NotBeNull();
        }

        [Fact]
        public void GetState_OfInterface_WhenStateIsSetToReadonlyAnyAttemptsToEditStateShouldThrow()
        {
            //Arrange
            var typeOfInterface = typeof(MyInterface);
            var state = sut.GetState(typeOfInterface) as MyInterface;
            (state as ICanSwitchBackAndToReadOnly).CanEdit = false;

            //Act
            var exception = Record.Exception(() => state.MyValue = 5);

            //Assert
            exception.Should().NotBeNull();
        }

        [Fact]
        public void GetState_OfInterface_WhenStateIsSetToMutableAnyAttemptToChangeStateShouldSucceed()
        {
            //Arrange
            var typeOfInterface = typeof(MyInterface);
            var state = sut.GetState(typeOfInterface) as MyInterface;
            (state as ICanSwitchBackAndToReadOnly).CanEdit = true;
            int value = 42;

            //Act
            var exception = Record.Exception(() => state.MyValue = value);

            //Assert
            exception.Should().BeNull();
            state.MyValue.Should().Be(value);
        }
    }
}
