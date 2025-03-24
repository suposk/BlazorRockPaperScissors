namespace MudBlazorRokPaperScissors.UnitTest.Domain;

public class PlayerTest : BaseTest
{

    [Fact]
    public void Create_Palyer_Ok()
    {
        //areange
        var player = new Player(PlayerType.Human, "Human", null);

        //act
        //request.Create(UtcNow);

        //assert
        //Assert.NotNull(player);
        player.Should().NotBeNull();
        player.PlayerType.Should().Be(PlayerType.Human);
        player.Name.Should().Be("Human");
        player.Id.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void Create_Player_Throws_Exception_When_Name_Is_Null()
    {
        // arrange
        Action act = () => new Player(PlayerType.Human, null, null);

        // act & assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Create_Player_Throws_Exception_When_Name_Is_Empty()
    {
        // arrange
        Action act = () => new Player(PlayerType.Human, string.Empty, null);

        // act & assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void SelectDraw_Rock_Should_Set_Draw_Property()
    {
        // arrange
        var player = BaseTest.PlayerHuman;
        var expectedDraw = DrawType.Rock;

        // act
        var result = player.SelectDraw(expectedDraw);

        // assert
        //result.Should().Be(Result.Success);
        result.IsSuccess.Should().BeTrue(); 
        player.Draw.Should().Be(expectedDraw);
    }

    [Fact]
    public void SelectDraw_Paper_Should_Set_Draw_Property()
    {
        // arrange
        var player = BaseTest.PlayerHuman;
        var expectedDraw = DrawType.Paper;

        // act
        var result = player.SelectDraw(expectedDraw);

        // assert
        result.IsSuccess.Should().BeTrue();
        player.Draw.Should().Be(expectedDraw);
    }

    [Fact]
    public void SelectDraw_Scissors_Should_Set_Draw_Property()
    {
        // arrange
        var player = BaseTest.PlayerHuman;
        var expectedDraw = DrawType.Scissors;

        // act
        var result = player.SelectDraw(expectedDraw);

        // assert
        result.IsSuccess.Should().BeTrue();
        player.Draw.Should().Be(expectedDraw);
    }

    [Fact]
    public void SelectDraw_None_Should_Fail()
    {
        // arrange
        var player = BaseTest.PlayerHuman;
        var expectedDraw = DrawType.None;

        // act
        var result = player.SelectDraw(expectedDraw);

        // assert
        result.IsSuccess.Should().BeFalse();        
    }

}
