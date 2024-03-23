using FluentAssertions;
using JetBrains.Annotations;
using NUnit.Framework;
using Xunit;

namespace Rogero.Common.Tests;

[TestFixture]
[TestSubject(typeof(NoConcurrentInvocations))]
public class NoConcurrentInvocationsTest
{
   [Test]
   public async Task OnlyOneShouldRun()
   {
      var sut      = new NoConcurrentInvocations();
      var runCount = 0;
      var action = async () =>
      {
         await Task.Delay(400);
         runCount++;
      };

      var task1 = Task.Run(() => sut.Run(action));
      var task2 = Task.Run(() => sut.Run(action));

      await Task.Delay(800);
      await Task.WhenAll(task1, task2);

      var t1 = await task1;
      var t2 = await task2;
      Console.WriteLine($"Task1: {t1}");
      Console.WriteLine($"Task2: {t2}");

      t1.Should().Be(NoConcurrentExecutionStatus.Ran);
      t2.Should().Be(NoConcurrentExecutionStatus.AlreadyRunning);

      runCount.Should().Be(1);
   }

   [Test]
   public async Task OnlyTwoShouldRun()
   {
      var sut      = new NoConcurrentInvocations();
      var runCount = 0;
      var action = async () =>
      {
         await Task.Delay(400);
         runCount++;
      };

      var task1 = Task.Run(() => sut.Run(action));
      await Task.Delay(400);
      var task2 = Task.Run(() => sut.Run(action));

      await Task.WhenAll(task1, task2);

      var t1 = await task1;
      var t2 = await task2;
      Console.WriteLine($"Task1: {t1}");
      Console.WriteLine($"Task2: {t2}");

      t1.Should().Be(NoConcurrentExecutionStatus.Ran);
      t2.Should().Be(NoConcurrentExecutionStatus.Ran);

      runCount.Should().Be(2);
   }

   [Fact]
   public async Task ShouldWork()
   {
      var sut      = new NoConcurrentInvocations();
      var runCount = 0;

      var task1 = Task.Run(() => sut.Run(async () =>
      {
         runCount++;
         await Task.Delay(1000);
      }));
      await Task.Delay(200);
      var task2 = Task.Run(() => sut.Run(() => { runCount++; }));
      await Task.Delay(200);
      var task3 = Task.Run(() => sut.Run(() => { runCount++; }));
      await Task.Delay(1000);
      var task4 = Task.Run(() => sut.Run(() => { runCount++; }));

      await Task.WhenAll(task1, task2, task3, task4);


      task1.Result.Should().Be(NoConcurrentExecutionStatus.Ran);
      task2.Result.Should().Be(NoConcurrentExecutionStatus.AlreadyRunning);
      task3.Result.Should().Be(NoConcurrentExecutionStatus.AlreadyRunning);
      task4.Result.Should().Be(NoConcurrentExecutionStatus.Ran);
      runCount.Should().Be(2);
   }

   [Fact]
   public async Task ShouldWorkWithError()
   {
      var sut      = new NoConcurrentInvocations();
      var runCount = 0;

      var task1 = Task.Run(() => sut.Run(async () =>
      {
         runCount++;
         await Task.Delay(1000);
         throw new Exception();
      }));
      await Task.Delay(200);
      var task2 = Task.Run(() => sut.Run(() => { runCount++; }));
      await Task.Delay(200);
      var task3 = Task.Run(() => sut.Run(() => { runCount++; }));
      await Task.Delay(1000);
      var task4 = Task.Run(() => sut.Run(() => { runCount++; }));

      await Task.WhenAll(task1, task2, task3, task4);

      task1.Result.Should().Be(NoConcurrentExecutionStatus.Error);
      task2.Result.Should().Be(NoConcurrentExecutionStatus.AlreadyRunning);
      task3.Result.Should().Be(NoConcurrentExecutionStatus.AlreadyRunning);
      task4.Result.Should().Be(NoConcurrentExecutionStatus.Ran);
      runCount.Should().Be(2);
   }

   [Fact]
   public async Task ShouldWorkWithErrorOfT()
   {
      var sut      = new NoConcurrentInvocations();
      var runCount = 0;

      var task1 = Task.Run(() => sut.Run(async () =>
      {
         runCount++;
         await Task.Delay(1000);
         throw new Exception();
         return (object) 1;
      }));
      await Task.Delay(200);
      var task2 = Task.Run(() => sut.Run(() =>
      {
         runCount++;
         return (object) 2;
      }));
      await Task.Delay(200);
      var task3 = Task.Run(() => sut.Run(() =>
      {
         runCount++;
         return (object) 3;
      }));
      await Task.Delay(1000);
      var task4 = Task.Run(() => sut.Run(() =>
      {
         runCount++;
         return (object) 4;
      }));

      await Task.WhenAll(task1, task2, task3, task4);

      NoConcurrentExecutionResult<object>
         t1 = task1.Result,
         t2 = task2.Result,
         t3 = task3.Result,
         t4 = task4.Result;

      t1.NoConcurrentExecutionStatus.Should().Be(NoConcurrentExecutionStatus.Ran);
      t2.NoConcurrentExecutionStatus.Should().Be(NoConcurrentExecutionStatus.AlreadyRunning);
      t3.NoConcurrentExecutionStatus.Should().Be(NoConcurrentExecutionStatus.AlreadyRunning);
      t4.NoConcurrentExecutionStatus.Should().Be(NoConcurrentExecutionStatus.Ran);
      runCount.Should().Be(2);
   }
}