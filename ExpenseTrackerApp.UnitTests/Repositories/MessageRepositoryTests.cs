using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExpenseTrackerApp.Data;
using ExpenseTrackerApp.Data.Repositories;
using ExpenseTrackerApp.Data.Repositories.IRepsitories;
using ExpenseTrackerApp.Models;
using ExpenseTrackerApp.UnitTests.Helpers;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.InMemory;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using ExpenseTrackerApp.Models.ViewModels.BudgetViewModels;
using ExpenseTrackerApp.UnitTests.Helpers.AddDummyData;
using FluentAssertions;

namespace ExpenseTrackerApp.UnitTests.Repositories;

public class MessageRepositoryTests
{
    private DbContextOptions<ApplicationDbContext> CreateDbContextOptions()
    {
        return new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase($"TestDatabase_{Guid.NewGuid()}")
            .Options;
    }

    // contains message this month start
    [Fact]
    public void containsMessageThisMonthShouldReturnFalse()
    {
        // Arrane
        var options = CreateDbContextOptions();

        using (var context = new ApplicationDbContext(options))
        {
            var repository = new MessageRepository(context);

            // Act
            var result = repository.ContainsMessageThisMonth(
                "userid",
                "testmessage"
            );

            Assert.False(result);
        }
    }

    [Fact]
    public void containsMessageThisMonthShouldReturnTrue()
    {
        // Arrange
        var options = CreateDbContextOptions();
        using (var context = new ApplicationDbContext(options))
        {
            var repository = new MessageRepository(context);

            var messageDb = new Message()
            {
                Id = 1,
                Description = "testmessage",
                Date = DateTime.Now,
                IconBackground = "testicon",
                IconType = "testicon",
                ControllerLink = "testlink",
                ApplicationUser = new ApplicationUser() { ApplicationUserName = "testuser" }
            };

            context.Add(messageDb);
            context.SaveChanges();

            // Act
            var result = repository.ContainsMessageThisMonth(
                messageDb.ApplicationUserId,
                "testmessage"
            );

            // Assert
            Assert.True(result);
        }
    }

    [Fact]
    public void containsMessageThisMonthShouldReturnFalseWithFilledDb()
    {
        // Arrnge
        var options = CreateDbContextOptions();
        using (var context = new ApplicationDbContext(options))
        {
            var repository = new MessageRepository(context);

            var messageDb = new Message()
            {
                Id = 1,
                Description = "testmessage",
                Date = new DateTime(2024, 01, 01),
                IconBackground = "testicon",
                IconType = "testicon",
                ControllerLink = "testlink",
                ApplicationUser = new ApplicationUser() { ApplicationUserName = "testuser" }
            };

            context.Add(messageDb);
            context.SaveChanges();

            var result = repository.ContainsMessageThisMonth
            (
                messageDb.ApplicationUserId,
                "testmessage"
            );

            // Assert
            Assert.False(result);
        }
    }

    // contains message this month end

    // get all messages start
    [Fact]
    public void getAllMessagesShouldReturnAllMessages()
    {
        // Arrange
        var options = CreateDbContextOptions();
        using (var context = new ApplicationDbContext(options))
        {
            var repository = new MessageRepository(context);

            var messageDb = new Message()
            {
                Id = 1,
                Description = "testmessage",
                Date = new DateTime(2024, 01, 01),
                IconBackground = "testicon",
                IconType = "testicon",
                ControllerLink = "testlink",
                ApplicationUser = new ApplicationUser() { ApplicationUserName = "testuser" }
            };

            context.Add(messageDb);
            context.SaveChanges();

            // Act
            var result = repository.GetAllMessages(messageDb.ApplicationUserId);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            result.First().Should().BeEquivalentTo(messageDb);
        }
    }

    [Fact]
    public void getAllMessagesShouldReturnNoMessages()
    {
        // Arrange
        var options = CreateDbContextOptions();
        using (var context = new ApplicationDbContext(options))
        {
            var repository = new MessageRepository(context);

            // Act
            var result = repository.GetAllMessages("testuser");

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }
    }
    // get all messages end

    // get recent messages
    [Fact]
    public void getRecentMessagesShouldReturnOneMessage()
    {
        // Arrange
        var options = CreateDbContextOptions();
        using (var context = new ApplicationDbContext(options))
        {
            var repository = new MessageRepository(context);

            var messageDb = new Message()
            {
                Id = 1,
                Description = "testmessage",
                Date = DateTime.Now,
                IconBackground = "testicon",
                IconType = "testicon",
                ControllerLink = "testlink",
                ApplicationUser = new ApplicationUser() { ApplicationUserName = "testuser" }
            };

            context.Add(messageDb);
            context.SaveChanges();

            // Act
            var result = repository.GetRecentMessages(messageDb.ApplicationUserId);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            result.First().Should().BeEquivalentTo(messageDb);
        }
    }

    [Fact]
    public void getRecentMessagesShouldReturnNoMessages()
    {
        // Arrange
        var options = CreateDbContextOptions();
        using (var context = new ApplicationDbContext(options))
        {
            var repository = new MessageRepository(context);
            
            var messageDb = new Message()
            {
                Id = 1,
                Description = "testmessage",
                Date = new DateTime(2024, 01, 01),
                IconBackground = "testicon",
                IconType = "testicon",
                ControllerLink = "testlink",
                ApplicationUser = new ApplicationUser() { ApplicationUserName = "testuser" }
            };

            context.Add(messageDb);
            context.SaveChanges();
            
            // Act
            var result = repository.GetRecentMessages(messageDb.ApplicationUserId);
            
            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }
    }
    
    [Fact]
    public void getRecentMessagesNoMessagesInDbShouldReturnNoMessages()
    {
        // Arrange
        var options = CreateDbContextOptions();
        using (var context = new ApplicationDbContext(options))
        {
            var repository = new MessageRepository(context);
            
            // Act
            var result = repository.GetRecentMessages("testuser");
            
            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }
    }
    // get recent messages end

    // get by id start
    [Fact]
    public void getByIdShouldReturnOneMessage()
    {
        // Arrange
        var options = CreateDbContextOptions();
        using (var context = new ApplicationDbContext(options))
        {
            var repository = new MessageRepository(context);
            
            var messageDb = new Message()
            {
                Id = 1,
                Description = "testmessage",
                Date = DateTime.Now,
                IconBackground = "testicon",
                IconType = "testicon",
                ControllerLink = "testlink",
                ApplicationUser = new ApplicationUser() { ApplicationUserName = "testuser" }
            };

            context.Add(messageDb);
            context.SaveChanges();
            
            // Act
            var result = repository.GetById(messageDb.Id);
            
            // Assert
            Assert.NotNull(result);
            result.Should().BeEquivalentTo(messageDb);
        }
    }
    
    [Fact]
    public void getByIdWrongInputIdShouldReturnNoMessage()
    {
        // Arrange
        var options = CreateDbContextOptions();
        using (var context = new ApplicationDbContext(options))
        {
            var repository = new MessageRepository(context);
            
            var messageDb = new Message()
            {
                Id = 1,
                Description = "testmessage",
                Date = DateTime.Now,
                IconBackground = "testicon",
                IconType = "testicon",
                ControllerLink = "testlink",
                ApplicationUser = new ApplicationUser() { ApplicationUserName = "testuser" }
            };

            context.Add(messageDb);
            context.SaveChanges();
            
            // Act
            var result = repository.GetById(messageDb.Id + 1);
            
            // Assert
            Assert.Null(result);
        }
    }
    
    [Fact]
    public void getByIdNoMessageInDbShouldReturnNoMessage()
    {
        // Arrange
        var options = CreateDbContextOptions();
        using (var context = new ApplicationDbContext(options))
        {
            var repository = new MessageRepository(context);
            
            // Act
            var result = repository.GetById(1);
            
            // Assert
            Assert.Null(result);
        }
    }
    // get by id end
    
    // create message with user id start
    [Fact]
    public void createMessageWithUserIdSuccess()
    {
        // Arrange
        var options = CreateDbContextOptions();
        using (var context = new ApplicationDbContext(options))
        {
            var repository = new MessageRepository(context);
            
            // Act
            repository.CreateMessageWithUserId
            (
                "testuser",
                "testmessage",
                "black",
                "linktype",
                "testlink",
                "linkaction"
                );
            
            // Assert
            var messageDb = context.messages.First();
            messageDb.Should().NotBeNull();
            messageDb.Description.Should().Be("testmessage");
        }
    }
    // crete message with user id end
    
    // delete message start
    [Fact]
    public void deleteMessageWithUserIdSuccess()
    {
        var options = CreateDbContextOptions();
        using (var context = new ApplicationDbContext(options))
        {
            var repository = new MessageRepository(context);
            
            var messageDb = new Message()
            {
                Id = 1,
                Description = "testmessage",
                Date = DateTime.Now,
                IconBackground = "testicon",
                IconType = "testicon",
                ControllerLink = "testlink",
                ApplicationUser = new ApplicationUser() { ApplicationUserName = "testuser" }
            };

            context.Add(messageDb);
            context.SaveChanges();
            
            // Act
            repository.DeleteMessage(messageDb);
            
            // Assert
            var message = context.messages.FirstOrDefault(m => m.Id == messageDb.Id);
            Assert.Null(message);
        }
    }
    
    [Fact]
    public void deleteNoMessageInDbArgumentWithUserIdFail()
    {
        var options = CreateDbContextOptions();
        using (var context = new ApplicationDbContext(options))
        {
            var repository = new MessageRepository(context);
            
            var messageDb = new Message()
            {
                Id = 1,
                Description = "testmessage",
                Date = DateTime.Now,
                IconBackground = "testicon",
                IconType = "testicon",
                ControllerLink = "testlink",
                ApplicationUser = new ApplicationUser() { ApplicationUserName = "testuser" }
            };
            
            // Act
            var exception = Assert.Throws<Exception>(() => repository.DeleteMessage(messageDb));
            
            // Assert
            exception.Message.Should().Be($"Message with id {messageDb.Id} does not exist");
        }
    }
    // delete message end
    
    // get message create in the current minute start
    [Fact]
    public void getMessageCreateInTheCurrentMinute()
    {
        // Arrange
        var options = CreateDbContextOptions();
        using (var context = new ApplicationDbContext(options))
        {
            var repository = new MessageRepository(context);
            
            var messageDb = new Message()
            {
                Id = 1,
                Description = "testmessage",
                Date = DateTime.UtcNow,
                IconBackground = "testicon",
                IconType = "testicon",
                ControllerLink = "testlink",
                ApplicationUser = new ApplicationUser() { ApplicationUserName = "testuser" }
            };

            context.Add(messageDb);
            context.SaveChanges();
            
            // Act
            var result = repository.GetMessageCreateInTheCurrentMinute(messageDb.ApplicationUserId);
            
            Assert.NotNull(result);
            result.First().Should().BeEquivalentTo(messageDb);
        }
    }
    
    [Fact]
    public void getMessageCreateInTheCurrentMinuteNoMatchingEntry()
    {
        // Arrange
        var options = CreateDbContextOptions();
        using (var context = new ApplicationDbContext(options))
        {
            var repository = new MessageRepository(context);
            
            var messageDb = new Message()
            {
                Id = 1,
                Description = "testmessage",
                Date = new DateTime(2024, 01, 01),
                IconBackground = "testicon",
                IconType = "testicon",
                ControllerLink = "testlink",
                ApplicationUser = new ApplicationUser() { ApplicationUserName = "testuser" }
            };

            context.Add(messageDb);
            context.SaveChanges();
            
            // Act
            var result = repository.GetMessageCreateInTheCurrentMinute(messageDb.ApplicationUserId);
            
            Assert.NotNull(result);
            Assert.Empty(result);
        }
    }
    
    [Fact]
    public void getMessageCreateInTheCurrentMinuteDbIsEmpty()
    {
        // Arrange
        var options = CreateDbContextOptions();
        using (var context = new ApplicationDbContext(options))
        {
            var repository = new MessageRepository(context);
            
            var messageDb = new Message()
            {
                Id = 1,
                Description = "testmessage",
                Date = new DateTime(2024, 01, 01),
                IconBackground = "testicon",
                IconType = "testicon",
                ControllerLink = "testlink",
                ApplicationUser = new ApplicationUser() { ApplicationUserName = "testuser" }
            };
            
            // Act
            var result = repository.GetMessageCreateInTheCurrentMinute(messageDb.ApplicationUserId);
            
            Assert.NotNull(result);
            Assert.Empty(result);
        }
    }
    
    // get message creat in the current minute end
}