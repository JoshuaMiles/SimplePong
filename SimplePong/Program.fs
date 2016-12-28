open System
open System.Windows.Forms
open System.Drawing
open PlayerFunctions
open BallFunctions

let WIDTH = 600
let HEIGHT = 400
let SCALE = 10

type myForm() =
    inherit Form()
    do base.DoubleBuffered <- true

let form = new myForm(Text = "Pong", ClientSize = Size(WIDTH, HEIGHT))

let gr = form.CreateGraphics()

let round (x:float) = int (System.Math.Round x)

let player1 =  { pos = P(50,50) ; dir = Direction(5.0,5.0) ; height = 100 ; width = 20 }

let player2 = { pos = P(250,250) ; dir = Direction(5.0,5.0); height = 100 ; width = 20 }

let drawDebugStrings (x:int, y:int, dy:float, dx: float, playerOrBall:String) = 
    use blue = new SolidBrush(Color.Blue)
    use black = new SolidBrush(Color.Black)
    use font = new Font("Arial", 16.0f)
    gr.DrawString(playerOrBall +  " x: " + x.ToString(), font, blue, PointF(0.0f,0.0f))
    gr.DrawString(playerOrBall + " y: " + y.ToString(), font, blue, PointF(0.0f,25.0f))
    gr.DrawString(playerOrBall + " dy: " + dy.ToString(), font, blue, PointF(100.0f,0.0f))
    gr.DrawString(playerOrBall + " dx: " + dx.ToString(), font, blue, PointF(100.0f,25.0f))

let debuggerDraw (gameBall: BallFunctions.GameObject, player1: BallFunctions.GameObject, player2 : BallFunctions.GameObject, e: KeyEventArgs) =
    match e.KeyValue with 
    | 38 ->  drawDebugStrings(gameBall.pos.X,gameBall.pos.Y, gameBall.dir.DY, gameBall.dir.DX, "Ball")
    | 40 ->  drawDebugStrings(gameBall.pos.X,gameBall.pos.Y, gameBall.dir.DY, gameBall.dir.DX, "Player 1")
    | 37 ->  drawDebugStrings(player2.pos.X,player2.pos.Y, player2.dir.DY, player2.dir.DX, "Player 2")
    | _ ->  drawDebugStrings(0,0,0.0,0.0,"")

let draw (gameBall: BallFunctions.GameObject) =
    use blue = new SolidBrush(Color.Blue)
    use black = new SolidBrush(Color.Black)
    use font = new Font("Arial", 16.0f)
    gr.FillRectangle(black, 0, 0, WIDTH, HEIGHT)
    gr.FillRectangle(blue, gameBall.pos.X, gameBall.pos.Y, gameBall.height,gameBall.width)

let drawPlayers (player1:BallFunctions.GameObject,player2:BallFunctions.GameObject) =
    use blue = new SolidBrush(Color.Blue)
    use black = new SolidBrush(Color.Black)
    gr.FillRectangle(blue, player1.pos.X, player1.pos.Y, 10, 10)
    gr.FillRectangle(blue, player2.pos.X, player2.pos.Y, 10, 10)


let startingAngle (dir: BallFunctions.Direction,degrees:float) =
    let radians = degrees *  (System.Math.PI / 180.0)
    let s = System.Math.Sin radians
    let c = System.Math.Cos radians
    let dx = c * dir.DX + s * dir.DY 
    let dy = -s * dir.DX + c * dir.DY
    BallFunctions.Direction(dy,dx)


let moveBall (gameBall : BallFunctions.GameObject) =   
    { pos = P(gameBall.pos.X + round gameBall.dir.DX, gameBall.pos.Y + round gameBall.dir.DY); 
    dir = Direction(gameBall.dir.DX, gameBall.dir.DY); width = gameBall.width; height = gameBall.height}

let rec gameLoop((gameBall: BallFunctions.GameObject , player1: BallFunctions.GameObject , player2 : BallFunctions.GameObject)) = async {
    let now = DateTime.Now
    let dt = 64.0 - DateTime.Now.Subtract(now).TotalMilliseconds
    // Apply the function to change the position of the ball to the ball
    let newBall = gameBall |> moveBall |> bounce
    draw newBall
    drawPlayers (player1, player2)
    do! Async.Sleep(Math.Max(dt, 0.0) |> int)
    return! gameLoop(newBall, player1, player2) }

let gameBall = {pos = P((WIDTH/2)-5,(HEIGHT/2)-5); dir = startingAngle(Direction(5.0,5.0), 30.0) ; height = 10; width = 10 }

[<STAThread>]
do Async.Start(gameLoop(gameBall, player1, player2))
   Application.Run(form)
