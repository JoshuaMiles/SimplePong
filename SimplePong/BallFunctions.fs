module BallFunctions

let WIDTH = 600
let HEIGHT = 400
let SCALE = 10


type P(x: int, y: int) =
    member this.X = x
    member this.Y = y

type Direction(dy: float, dx: float) =
    member this.DY = dy
    member this.DX = dx

type GameObject = {pos: P ; dir : Direction; height: int; width: int}


let bounce (gameBall: GameObject) =
    let checkBounds min max currentPos dir =
        match currentPos with
            | currentPos when currentPos <= min -> -dir
            | currentPos when currentPos > max - 10 -> -dir
            | _ -> dir
    let dx = checkBounds 0 WIDTH gameBall.pos.X gameBall.dir.DY
    let dy = checkBounds 0 HEIGHT gameBall.pos.Y gameBall.dir.DX
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



//bool sprite_collision(sprite_id sprite_1, sprite_id sprite_2) {
//
//    int sprite_1_left = (int) round(sprite_x(sprite_1)),
//            sprite_1_right = sprite_1_left + sprite_width(sprite_1),
//            sprite_1_top = (int) round(sprite_y(sprite_1)),
//            sprite_1_bottom = sprite_1_top + sprite_height(sprite_1),
//
//            sprite_2_left = (int) round(sprite_x(sprite_2)),
//            sprite_2_right = sprite_2_left + sprite_width(sprite_2),
//            sprite_2_top = (int) round(sprite_y(sprite_2)),
//            sprite_2_bottom = sprite_2_top + sprite_height(sprite_2);
//
//    return
//            sprite_1_bottom > sprite_2_top &&
//            sprite_1_top < sprite_2_bottom &&
//            sprite_1_left < sprite_2_right &&
//            sprite_1_right > sprite_2_left;
//}