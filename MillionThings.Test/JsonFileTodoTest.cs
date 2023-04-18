using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using MillionThings;

namespace MillionThings.Test
{
    public class JsonFileTodoTest
    {
        [Fact]
        public void ShouldInstantiateWithExistingJsonFile()
        {
            Todo sut = new JsonFileTodo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "onetodo.json"));
        }

        [Fact]
        public void ShouldInstantiateAndCreateNewFile()
        {
            string filename = Guid.NewGuid().ToString();
            Todo sut = new JsonFileTodo(filename);
        }

        [Fact]
        public void ShouldListTodoFromGivenJsonFile()
        {
            Todo sut = new JsonFileTodo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "onetodo.json"));
            List<TodoItem> expected = new() { new() { Id = "1", Description = "This is the first todo" } };
            Assert.Equal(expected, sut.List());
        }

        [Fact]
        public void ShouldListTodosFromGivenJsonFile()
        {
            Todo sut  = new JsonFileTodo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "twotodos.json"));
            List<TodoItem> expected = new() { new() { Id = "1", Description = "This is the first todo" }, new() { Id = "2", Description = "This is the second todo" } };
            Assert.Equal(expected, sut.List());
        }

        [Fact]
        public void ShouldAddTodo()
        {
            Todo sut = CreateRandomTodo();
            sut.Add("Write some code that doesn't suck.");

            Assert.Contains(sut.List(), item => item.Description == "Write some code that doesn't suck.");
        }

        [Fact]
        public void ShouldNotListTodosThatAreDone()
        {
            Todo sut = CreateRandomTodo();
            sut.Add("This is my one and only todo");
            Assert.Contains(sut.List(), item => item.Description == "This is my one and only todo");
            sut.Done(sut.List()[0].Id);
            Assert.DoesNotContain(sut.List(), item => item.Description == "This is my one and only todo");
        }

        [Fact]
        public void ShouldNotFailWhenSettingANonExistentTodoToDone()
        {
            Todo sut = CreateRandomTodo();
            sut.Add("This is my one and only todo");
            Assert.Contains(sut.List(), item => item.Description == "This is my one and only todo");
            sut.Done("SOME_WRONG_ID");
            Assert.Contains(sut.List(), item => item.Description == "This is my one and only todo");
        }

        [Fact]
        public void ShouldPersistTodosInJsonFile()
        {
            string filename = Guid.NewGuid().ToString();
            Todo sut = new JsonFileTodo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filename));
            Assert.Empty(sut.List());
            sut.Add("Testing One");
            sut.Add("Testing Two");
            Assert.Contains(sut.List(), item => item.Description == "Testing One");
            Assert.Contains(sut.List(), item => item.Description == "Testing Two");
            sut.Done(sut.List().First().Id);
            Assert.DoesNotContain(sut.List(), item => item.Description == "Testing One");

            sut = null;

            Todo reloadedSut = new JsonFileTodo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filename));
            Assert.DoesNotContain(reloadedSut.List(), item => item.Description == "Testing One");
            Assert.Contains(reloadedSut.List(), item => item.Description == "Testing Two");
        }

        [Fact]
        public void ShouldNotAddAlreadyExistingTodo()
        {
            Todo sut = CreateRandomTodo();

            sut.Add("Testing One");
            List<TodoItem> expected = new() { new() { Id = "1", Description = "This is the first todo" } };
            Assert.Single(sut.List());

            sut.Add("Testing One");
            Assert.Single(sut.List());
            Assert.Contains(sut.List(), item => item.Description == "Testing One");
        }

        [Fact]
        public void ShouldUpdateTodoDescription()
        {
            Todo sut = CreateRandomTodo();

            sut.Add("Testing One");
            TodoItem savedTodo = sut.List().First();

            sut.Update(new() { Id = savedTodo.Id, Description = "Updated todo description" });
            Assert.Single(sut.List());
            Assert.DoesNotContain(sut.List(), item => item.Description == "Testing One");
            Assert.Contains(sut.List(), item => item.Description == "Updated todo description");
        }

        [Fact]
        public void ShouldAddTodoWhenUpdateTodoHasADifferentId()
        {
            Todo sut = CreateRandomTodo();

            sut.Add("Testing One");

            sut.Update(new() { Id = "ID_DOES_NOT_EXIST", Description = "Updated todo description" });
            Assert.Equal(2, sut.List().Count());
            Assert.Contains(sut.List(), item => item.Description == "Testing One");
            Assert.Contains(sut.List(), item => item.Description == "Updated todo description");
        }

        [Fact]
        public void ShouldNotAddTodoWhenUpdateTodoHasADifferentIdButSameDescriptionAlreadyExists()
        {
            Todo sut = CreateRandomTodo();

            sut.Add("Testing One");

            sut.Update(new() { Id = "ID_DOES_NOT_EXIST", Description = "Testing One" });
            Assert.Single(sut.List());
            Assert.Contains(sut.List(), item => item.Description == "Testing One");
        }

        private static Todo CreateRandomTodo()
        {
            string filename = Guid.NewGuid().ToString();
            return new JsonFileTodo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filename));
        }
    }
}
