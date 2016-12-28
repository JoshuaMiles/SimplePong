
module ObjectFunctions
open System.Windows.Forms


let WIDTH = 600
let HEIGHT = 400


type P(x: int, y: int) =
    member this.X = x
    member this.Y = y

type Direction(dy: float, dx: float) =
    member this.DY = dy
    member this.DX = dx

type GameObject = {pos: P ; dir : Direction; height: int; width: int}

let round (x:float) = int (System.Math.Round x)

let startingAngle (dir: Direction,degrees:float) =
    let radians = degrees *  (System.Math.PI / 180.0)
    let s = System.Math.Sin radians
    let c = System.Math.Cos radians
    let dx = c * dir.DX + s * dir.DY 
    let dy = -s * dir.DX + c * dir.DY
    Direction(dy,dx)

// A function that takes the game object and than applies the DY and DX variables to the current position effectivly moving it
let moveObject (gameBall : GameObject) =   
    { pos = P(gameBall.pos.X + round gameBall.dir.DX, gameBall.pos.Y + round gameBall.dir.DY); 
    dir = Direction(gameBall.dir.DY, gameBall.dir.DX); width = gameBall.width; height = gameBall.height}

// Keeps the ball within the bounds of the game screeen, going to be used to 
let perimeter (gameBall: GameObject) =
    let checkBounds max currentPos dir ballDimension =
        match currentPos with
            | currentPos when currentPos <= 0 -> -dir
            | currentPos when currentPos > max - ballDimension -> -dir
            | _ -> dir
    let dx = checkBounds WIDTH gameBall.pos.X gameBall.dir.DX gameBall.width
    let dy = checkBounds HEIGHT gameBall.pos.Y gameBall.dir.DY gameBall.height
    // Less graceful solution but a common problem with immutability
    {pos = P(gameBall.pos.X,gameBall.pos.Y); dir = Direction(dy, dx); height = gameBall.height; width = gameBall.width }

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

let checkAndLetTheObjectBounce (gameBall: GameObject, player1: GameObject, player2) =
    if collision (gameBall, player1) || collision (gameBall, player2)
    then {pos = P(gameBall.pos.X, gameBall.pos.Y); dir = Direction(gameBall.dir.DY, -gameBall.dir.DX); height = gameBall.height; width = gameBall.width }
    else  gameBall



let keyToDir = function
    | 38 -> Some(Direction(-5.0,0.0))
    | 40 -> Some(Direction(5.0,0.0))
    | _  -> None

let mutable morphableDirection = Direction(0.0,0.0)

// KeyEventArgs => unit
let morphMutableDirVariable(e: KeyEventArgs) =
    let dir = keyToDir e.KeyValue
    if Option.isSome dir
    then morphableDirection <- Option.get dir
    else morphableDirection <- Direction(0.0,0.0)

let stopThePaddle(e:KeyEventArgs) =
    morphableDirection <- Direction(0.0,0.0)


// GameObject => Direction => GameObject
let combiningPlayerAndDirection(player: GameObject) = 
    let checkPlayerBounds height currentPos dyDir =
       match currentPos with
           | currentPos when currentPos + round dyDir  <= 0 ->   {pos = player.pos ; dir = Direction(0.0,0.0) ; height = player.height ; width = player.width}
           | currentPos when currentPos + round dyDir + height > HEIGHT ->  {pos = player.pos ; dir = Direction(0.0,0.0) ; height = player.height ; width = player.width}
           | _ ->  {pos = player.pos ; dir = morphableDirection ; height = player.height ; width = player.width}
    checkPlayerBounds player.height player.pos.Y player.dir.DY