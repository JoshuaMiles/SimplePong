module ObjectFunctions
open System.Windows.Forms
open System.Drawing

let WIDTH = 1800
let HEIGHT = 1200

type P(x: int, y: int) =
    member this.X = x
    member this.Y = y

type Direction(dy: float, dx: float) =
    member this.DY = dy
    member this.DX = dx

type GameObject = {pos: P ; dir : Direction; width:int ; height: int; currentColour:int}

let listOfColours = [| "#551a8b" ; "#FF69B4" ; "#ff0000" ; "#ff8d00" ; "#ffff00" ; "#00ff00" ; "#00ecff" |]

let round (x:float) = int (System.Math.Round x)

let startingAngle (dir: Direction) =
    let rand = System.Random()
    let radians = 360.0 * rand.NextDouble()
    let s = System.Math.Sin radians
    let c = System.Math.Cos radians
    let dx = c * dir.DX + s * dir.DY
    let dy = -s * dir.DX + c * dir.DY
    Direction(dy,dx)

// A function that takes the game object and than applies the DY and DX variables to the current position effectivly moving it
// GameObject => GameObject
let moveObject (gameObject : GameObject) =   
    { pos = P(gameObject.pos.X + round gameObject.dir.DX, gameObject.pos.Y + round gameObject.dir.DY); 
    dir = Direction(gameObject.dir.DY, gameObject.dir.DX); width = gameObject.width; height = gameObject.height; currentColour = gameObject.currentColour }

// Keeps the ball within the bounds of the game screeen, going to be used to 
// GameObject => GameObject
let perimeter (gameBall: GameObject) =
    let checkBounds max currentPos dir ballDimension =
        match currentPos with
            | curentPos when (currentPos <= 0) || (currentPos > max - ballDimension) -> -dir
            | _ -> dir
    let dx = checkBounds WIDTH gameBall.pos.X gameBall.dir.DX gameBall.width
    let dy = checkBounds HEIGHT gameBall.pos.Y gameBall.dir.DY gameBall.height
    // Less graceful solution but a common problem with immutability
    {pos = P(gameBall.pos.X,gameBall.pos.Y); dir = Direction(dy, dx) ; width = gameBall.width ; height = gameBall.height; currentColour = gameBall.currentColour }

// (GameObject => GameObject) => Bool
let collision (gameBall: GameObject, player : GameObject) =
    let gameBallLeft = gameBall.pos.X
    let gameBallRight = gameBallLeft + gameBall.width
    let gameBallTop = gameBall.pos.Y
    let gameBallBottom = gameBallTop + gameBall.height
    
    let playerLeft = player.pos.X
    let playerRight = playerLeft + player.width
    let playerTop = player.pos.Y
    let playerBottom = playerTop + player.height

    gameBallBottom > playerTop && 
    gameBallTop < playerBottom &&
    gameBallLeft < playerRight &&
    gameBallRight > playerLeft 


// (GameObject => GameObject) => GameObject
let checkAndLetTheObjectBounce (gameBall: GameObject, player1: GameObject, player2: GameObject) =
    if collision (gameBall, player1) || collision (gameBall, player2)
    then {pos = P(gameBall.pos.X, gameBall.pos.Y); dir = startingAngle(Direction(gameBall.dir.DY, -gameBall.dir.DX)) ; width = gameBall.width ; height = gameBall.height; currentColour = (gameBall.currentColour + 1)%7 }
    else  gameBall

let checkToChangeColourOfPaddle(gameBall: GameObject,  player: GameObject) =
    if collision (gameBall, player)
    then {pos = P(player.pos.X, player.pos.Y); dir = player.dir ; width = player.width ; height = player.height; currentColour = (player.currentColour + 1)%7 }
    else  player

// int => Direction Option
let keyToDir = function
    | 38 -> Some(Direction(-5.0,0.0))
    | 40 -> Some(Direction(5.0,0.0))
    | _  -> None

let mutable morphableDirection = Direction(0.0,0.0)

 // Need to make a function which creates a side effect that changes the direction of the paddle from the input
// KeyEventArgs => unit
let morphMutableDirVariable(e: KeyEventArgs) =
    let dir = keyToDir e.KeyValue
    if Option.isSome dir
    then morphableDirection <- Option.get dir
    else morphableDirection <- Direction(0.0,0.0)

// Whenever you see unit remember that it is just returning nothing
// KeyEventArgs => unit
let stopThePaddle(e:KeyEventArgs) =
    morphableDirection <- Direction(0.0,0.0)

// Paddle just kept moving, this is in place to default to stop. thought having the else clause in morphMutableDirVariable would do that
// GameObject => Direction => GameObject
let combiningPlayerAndDirection(player: GameObject) = 
    let checkPlayerBounds height currentPos =
       match currentPos with
           | currentPos when (currentPos + round morphableDirection.DY > 0) && (currentPos + round morphableDirection.DY + height < 400)  ->  {pos = player.pos ; dir = morphableDirection ; width = player.width ; height = player.height; currentColour = player.currentColour}
           | _ ->  {pos = player.pos ; dir = Direction(0.0,0.0) ; width = player.width ; height = player.height; currentColour = player.currentColour }
    checkPlayerBounds player.height player.pos.Y