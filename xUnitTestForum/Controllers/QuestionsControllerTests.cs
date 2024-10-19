using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Forum.Models;
using Forum.Data;
using Forum.Controllers;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using Moq.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;

namespace xUnitTestForum.Controllers
{
    public class QuestionsControllerTests
    {
        
      [Fact]
      public async Task TestIndex()
      {
          var questionList = new List<Question>()
          {
              new Question
              {
                  Id = 1,
                  Title = "What is mathematics?",
                  Description="Mathematics is the science that deals with the logic of shape, quantity and arrangement."
              },
              new Question
              {
                  Id=2,
                  Title="Mathematics",
                  Description="Algebraic geometry is the study of geometries that come from algebra, in particular"
              }
          };
          var mockDbContext = new Mock<ApplicationDbContext>(MockBehavior.Loose);
          mockDbContext.Setup(c => c.Questions)
                       .ReturnsDbSet(questionList);

          var mockLogger = new Mock<ILogger<QuestionsController>>();

          var controller = new QuestionsController(mockDbContext.Object, mockLogger.Object);

            // Act
           var result = await controller.Index();

           // Assert
           var viewResult = Assert.IsType<ViewResult>(result);
           Assert.NotNull(viewResult.ViewData.Model);

           var model = Assert.IsAssignableFrom<List<Question>>(viewResult.ViewData.Model);
           Assert.Equal(questionList.Count, model.Count);
      }

        [Fact]
        public async Task TestUpdate()
        {
            var testQuestion = new Question
            {
                Id = 1,
                Title = "Math",
                Description = "Description of the Combination",
                IdentityUserId = "userId1"
            };

            var mockDbContext = new Mock<ApplicationDbContext>(MockBehavior.Loose);
            mockDbContext.Setup(db => db.Questions.FindAsync(1)).ReturnsAsync(testQuestion);
            var mockLogger = new Mock<ILogger<QuestionsController>>();
            var controller = new QuestionsController(mockDbContext.Object, mockLogger.Object);

            // Act
            var result = await controller.Edit(1, testQuestion);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);

        }

        [Fact]
        public async Task TestUpdateNotOk()
        {
            // Arrange
            var testQuestion = new Question
            {
                Id = 1,
                Title = "Existing Question",
                Description = "Description of the existing question",
                IdentityUserId = "userId1"
            };

            var mockDbContext = new Mock<ApplicationDbContext>(MockBehavior.Loose);
            mockDbContext.Setup(db => db.Questions.FindAsync(1)).ReturnsAsync(testQuestion);
            var mockLogger = new Mock<ILogger<QuestionsController>>();
            var controller = new QuestionsController(mockDbContext.Object, mockLogger.Object);

            // Act
            var result = await controller.Edit(2, testQuestion); 

            // Assert
            var notFoundResult = Assert.IsType<NotFoundResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
        }





        [Fact]
      public async Task TestCreateNotOk()
      {
            var testQuestion = new Question
            {
                Id = 1,
                Title = "What is mathematics?",
                Description = "Mathematics is the science that deals with the logic of shape, arrangement."
            };

            var mockDbContext = new Mock<ApplicationDbContext>(MockBehavior.Loose);
            var mockLogger = new Mock<ILogger<QuestionsController>>();
            var controller = new QuestionsController(mockDbContext.Object, mockLogger.Object);

            // act
            var result = await controller.Create(testQuestion);

            // assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(QuestionsController.Index), redirectResult.ActionName);
      }
    }
}

