open System
open System.Windows.Forms
open System.Drawing
open ObjectFunctions
open PlayerFunctions

type myForm() =
    inherit Form()
    do base.DoubleBuffered <- true

let form = new myForm(Text = "Pong", ClientSize = Size(WIDTH, HEIGHT))

let gr = form.CreateGraphics()

let drawDebugStrings (x:int, y:int, dy:float, dx: float, playerOrBall:String) = 
    use blue = new SolidBrush(Color.Blue)
    use black = new SolidBrush(Color.Black)
    use font = new Font("Arial", 16.0f)
    gr.DrawString(playerOrBall +  " x: " + x.ToString(), font, blue, PointF(0.0f,0.0f))
    gr.DrawString(playerOrBall + " y: " + y.ToString(), font, blue, PointF(0.0f,25.0f))
    gr.DrawString(playerOrBall + " dy: " + dy.ToString(), font, blue, PointF(100.0f,0.0f))
    gr.DrawString(playerOrBall + " dx: " + dx.ToString(), font, blue, PointF(100.0f,25.0f))

let debuggerDraw (gameBall: ObjectFunctions.GameObject, player1: ObjectFunctions.GameObject, player2 : ObjectFunctions.GameObject, e: KeyEventArgs) =
    match e.KeyValue with 
    | 38 ->  drawDebugStrings(gameBall.pos.X,gameBall.pos.Y, gameBall.dir.DY, gameBall.dir.DX, "Ball")
    | 40 ->  drawDebugStrings(gameBall.pos.X,gameBall.pos.Y, gameBall.dir.DY, gameBall.dir.DX, "Player 1")
    | 37 ->  drawDebugStrings(player2.pos.X,player2.pos.Y, player2.dir.DY, player2.dir.DX, "Player 2")
    | _ ->  drawDebugStrings(0,0,0.0,0.0,"")

let draw (gameBall: ObjectFunctions.GameObject) =
    use blue = new SolidBrush(Color.Blue)
    use black = new SolidBrush(Color.Black)

    gr.FillRectangle(black, 0, 0, WIDTH, HEIGHT)
    gr.FillRectangle(blue, gameBall.pos.X, gameBall.pos.Y, gameBall.height,gameBall.width)

let drawPlayers (player1:ObjectFunctions.GameObject,player2:ObjectFunctions.GameObject) =
    use blue = new SolidBrush(Color.Blue)
    use black = new SolidBrush(Color.Black)
    use font = new Font("Arial", 16.0f)
    gr.DrawString("x: " + player1.pos.X.ToString(), font, blue, PointF(0.0f,0.0f))
    gr.DrawString("y: " + player1.pos.Y.ToString(), font, blue, PointF(0.0f,25.0f))
    gr.DrawString("dy: " + player1.dir.DY.ToString(), font, blue, PointF(100.0f,0.0f))
    gr.DrawString("dx: " + player1.dir.DX.ToString(), font, blue, PointF(100.0f,25.0f))
    gr.FillRectangle(blue, player1.pos.X, player1.pos.Y, 10, 10)
    gr.FillRectangle(blue, player2.pos.X, player2.pos.Y, 10, 10)



// The primary loop that the game keeps repeating, acting like the 'motor' of the program
let rec gameLoop((gameBall: ObjectFunctions.GameObject , player1: ObjectFunctions.GameObject , player2 : ObjectFunctions.GameObject)) = async {
    let now = DateTime.Now
    let dt = 64.0 - DateTime.Now.Subtract(now).TotalMilliseconds
    // Apply the function to change the position of the ball to the ball
    let newBall = checkAndLetTheObjectBounce (gameBall, player1, player2) |> moveObject |> bounce 
    // Current problem, the Objservable.add function needs to take a function which returns unit (which is just nothing)
    // Need to make a function which creates a side effect that changes the direction of the paddle from the input
    form.KeyDown |> Observable.add morphMutableDirVariable
    // Paddle just kept moving, this is in place to default to stop. thought having the else clause in morphMutableDirVariable would do that
    form.KeyUp |> Observable.add stopThePaddle
    let newPlayer1 = combiningPlayerAndDirection(player1,morphableDirection) |> moveObject

    draw newBall
    drawPlayers (newPlayer1, player2)

    do! Async.Sleep(Math.Max(dt, 0.0) |> int)
    return! gameLoop(newBall, newPlayer1, player2) }

let gameBall = {pos = P((WIDTH/2)-5,(HEIGHT/2)-5); dir = startingAngle(Direction(5.0,5.0), 30.0) ; height = 10; width = 10 }

let player1 =  { pos = P(50,50) ; dir = Direction(5.0,5.0) ; height = 100 ; width = 20 }

let player2 = { pos = P(550,250) ; dir = Direction(5.0,5.0); height = 100 ; width = 20 }

[<STAThread>]
do Async.Start(gameLoop(gameBall, player1, player2))
   Application.Run(form)
