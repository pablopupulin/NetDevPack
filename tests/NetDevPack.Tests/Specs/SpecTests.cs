using NetDevPack.Specification;
using System;
using System.Linq;
using Xunit;

namespace NetDevPack.Tests.Specs
{
    public class SpecTests
    {
        [Fact(DisplayName = "SingleSpecification ReturnTrue")]
        [Trait("Category", "Specification Tests")]
        public void Specification_SingleSpecification_ShouldReturnTrue()
        {
            // Arrange
            var movie = MovieFactory.GetForKids();

            var kidSpec = new MovieForKidsSpecification();

            // Act
            var result = kidSpec.IsSatisfiedBy(movie);

            // Assert
            Assert.True(result);
        }

        [Fact(DisplayName = "SingleSpecification ReturnFalse")]
        [Trait("Category", "Specification Tests")]
        public void Specification_SingleSpecification_ShouldReturnFalse()
        {
            // Arrange
            var movie = MovieFactory.GetRatedR();

            var kidSpec = new MovieForKidsSpecification();

            // Act
            var result = kidSpec.IsSatisfiedBy(movie);

            // Assert
            Assert.False(result);
        }

        [Fact(DisplayName = "AndSpecification")]
        [Trait("Category", "Specification Tests")]
        public void Specification_AndSpecification_ShouldReturnTrue()
        {
            // Arrange
            var movie = MovieFactory.GetMixedMovies().FirstOrDefault(m =>
                m.MpaaRating <= MpaaRating.PG && m.ReleaseDate.Year < DateTime.Now.Year);
            var director = movie.Director.Name;

            var kidSpec = new MovieForKidsSpecification();
            var dirSpec = new MovieDirectedBySpecification(director);
            var dvdSpec = new AvailableOnDvdSpecification();

            var movieSpec = kidSpec.And(dirSpec).And(dvdSpec);

            // Act
            var result = movieSpec.IsSatisfiedBy(movie);

            // Assert
            Assert.True(result);
        }

        [Fact(DisplayName = "OrSpecification")]
        [Trait("Category", "Specification Tests")]
        public void Specification_OrSpecification_ShouldFilterMovies()
        {
            // Arrange
            var movieCount = MovieFactory.GetMixedMovies().Count(m => m.MpaaRating <= MpaaRating.PG || m.Rating >= 4);

            var kidSpec = new MovieForKidsSpecification();
            var dirBest = new BestRatedFilmsSpecification();

            var movieSpec = kidSpec.Or(dirBest);

            // Act
            var result = MovieFactory.GetMixedMovies().Where(movieSpec.IsSatisfiedBy);

            // Assert
            Assert.Equal(movieCount, result.Count());
        }

        [Fact(DisplayName = "NotSpecification")]
        [Trait("Category", "Specification Tests")]
        public void Specification_NotSpecification_ShouldFilterMovies()
        {
            // Arrange
            var movieCount = MovieFactory.GetMixedMovies().Count(m => m.MpaaRating > MpaaRating.PG && m.Rating >= 4);

            var kidSpec = new MovieForKidsSpecification();
            var dirBest = new BestRatedFilmsSpecification();

            var movieSpec = dirBest.And(kidSpec.Not());

            // Act
            var result = MovieFactory.GetMixedMovies().Where(movieSpec.IsSatisfiedBy);

            // Assert
            Assert.Equal(movieCount, result.Count());
        }

        [Fact(DisplayName = "GenericSpecification")]
        [Trait("Category", "Specification Tests")]
        public void Specification_GenericSpecification_ShouldReturnTrue()
        {
            // Arrange
            var movie = MovieFactory.GetMixedMovies().FirstOrDefault(m => m.MpaaRating > MpaaRating.PG && m.Rating >= 4);
            var genSpec = new GenericSpecification<Movie>(m => m.MpaaRating > MpaaRating.PG && m.Rating >= 4);
            
            // Act
            var result = genSpec.IsSatisfiedBy(movie);

            // Assert
            Assert.True(result);
        }
    }
}
