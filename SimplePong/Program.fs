open System
open System.Windows.Forms
open System.Drawing

let WIDTH = 600
let HEIGHT = 400
let SCALE = 10

type myForm() =
    inherit Form()
    do base.DoubleBuffered <- true

type P(x: int, y: int) =
    member this.X = x
    member this.Y = y

type Direction(dy: float, dx: float) =
    member this.DY = dy
    member this.DX = dx

type Ball = {pos: P ; dir : Direction}

let form = new myForm(Text = "Pong", ClientSize = Size(WIDTH, HEIGHT))

let gr = form.CreateGraphics()

//let bounce()

let draw (gameBall: Ball) =
    use blue = new SolidBrush(Color.Blue)
    use black = new SolidBrush(Color.Black)
    gr.FillRectangle(black, 0, 0, WIDTH, HEIGHT)
    gr.FillRectangle(blue, gameBall.pos.X, gameBall.pos.Y, 10,10)


let startingAngle (dir:Direction,degrees:float) =
    let radians = degrees *  System.Math.PI
    let s = System.Math.Sin radians
    let c = System.Math.Cos radians
    let dx = c * dir.DX + s * dir.DY 
    let dy = -s * dir.DX + c * dir.DY
    Direction(dy,dx)

let bounce (gameBall: Ball) =
    let checkBounds min max currentPos dir =
        match currentPos with
            | currentPos when currentPos <= min+10  -> -dir
            | currentPos when currentPos >= max-5 -> -dir
            | _ -> dir
    let dx = checkBounds 0 WIDTH gameBall.pos.X gameBall.dir.DY
    let dy = checkBounds 0 HEIGHT gameBall.pos.Y gameBall.dir.DY
    {pos = P(gameBall.pos.X,gameBall.pos.Y); dir = Direction(dy, dx) }

let round (x:float) = int (System.Math.Round x)

let moveBall (gameBall:Ball) =   
    {pos = P(gameBall.pos.X + round gameBall.dir.DX, gameBall.pos.Y + round gameBall.dir.DY); 
    dir = Direction(gameBall.dir.DX, gameBall.dir.DY)}

let rec gameLoop((gameBall: Ball)) = async {
    let now = DateTime.Now
    let dt = 64.0 - DateTime.Now.Subtract(now).TotalMilliseconds
    // Apply the function to change the position of the ball to the ball
    let newBall = gameBall |> moveBall |> bounce
    draw newBall
    do! Async.Sleep(Math.Max(dt, 0.0) |> int)
    return! gameLoop(newBall) }

let gameBall = {pos = P((WIDTH/2)-5,(HEIGHT/2)-5); dir = startingAngle(Direction(5.0,5.0), 50.0) }

[<STAThread>]
do Async.Start(gameLoop(gameBall))
   Application.Run(form)
