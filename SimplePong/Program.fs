open System
open System.Windows.Forms
open System.Drawing
open ObjectFunctions
open PlayerFunctions

type myForm() =
    inherit Form()
    do base.DoubleBuffered <- true

let cConvert (colourString: string) = ColorTranslator.FromHtml(colourString);

let form = new myForm(Text = "Pong", ClientSize = Size(WIDTH, HEIGHT))

let gr = form.CreateGraphics()

let linkedListOfColours = [| "#551a8b" ; "#FF69B4" ; "#ff0000" ; "#ff8d00" ; "#ffff00" ; "#00ff00" ; "#00ecff" |]

// ctrl k c to comment ctrl k u to uncomment
//let drawDebugStrings (x:int, y:int, dy:float, dx: float, playerOrBall:String) = 
//    use blue = new SolidBrush(ColorTranslator.FromHtml("#551a8b"))
//    use black = new SolidBrush(Color.Black)
//    use font = new Font("Arial", 16.0f)
//    gr.DrawString(playerOrBall +  " x: " + x.ToString(), font, blue, PointF(0.0f,0.0f))
//    gr.DrawString(playerOrBall + " y: " + y.ToString(), font, blue, PointF(0.0f,25.0f))
//    gr.DrawString(playerOrBall + " dy: " + dy.ToString(), font, blue, PointF(100.0f,0.0f))
//    gr.DrawString(playerOrBall + " dx: " + dx.ToString(), font, blue, PointF(100.0f,25.0f))

//let debuggerDraw (gameBall: ObjectFunctions.GameObject, player1: ObjectFunctions.GameObject, player2 : ObjectFunctions.GameObject, e: KeyEventArgs) =
//    match e.KeyValue with 
//    | 38 ->  drawDebugStrings(gameBall.pos.X,gameBall.pos.Y, gameBall.dir.DY, gameBall.dir.DX, "Ball")
//    | 40 ->  drawDebugStrings(gameBall.pos.X,gameBall.pos.Y, gameBall.dir.DY, gameBall.dir.DX, "Player 1")
//    | 37 ->  drawDebugStrings(player2.pos.X,player2.pos.Y, player2.dir.DY, player2.dir.DX, "Player 2")
//    | _ ->  drawDebugStrings(0,0,0.0,0.0,"")

let draw (gameBall: ObjectFunctions.GameObject) =
    use ballColour =  new SolidBrush(cConvert linkedListOfColours.[gameBall.currentColour])
    use black = new SolidBrush(Color.Black)
    use white = new SolidBrush(Color.White)
    
    gr.FillRectangle(black, 0,0, WIDTH, HEIGHT)
    gr.FillRectangle(ballColour, gameBall.pos.X, gameBall.pos.Y, gameBall.height,gameBall.width)

let drawPlayers (player1:ObjectFunctions.GameObject,player2:ObjectFunctions.GameObject) =
    use player1Colour = new SolidBrush(cConvert linkedListOfColours.[player1.currentColour])
    use player2Colour = new SolidBrush(cConvert linkedListOfColours.[player2.currentColour])
    gr.FillRectangle(player1Colour, player1.pos.X, player1.pos.Y, player1.width, player1.height)
    gr.FillRectangle(player2Colour, player2.pos.X, player2.pos.Y, player2.width, player2.height)
  //  gr.DrawString("x: " + player1.pos.X.ToString(), font, blue, PointF(0.0f,0.0f))
   // gr.DrawString("y: " + player1.pos.Y.ToString(), font, blue, PointF(0.0f,25.0f))
   // gr.DrawString("dy: " + player1.dir.DY.ToString(), font, blue, PointF(100.0f,0.0f))
   // gr.DrawString("dx: " + player1.dir.DX.ToString(), font, blue, PointF(100.0f,25.0f))

// The primary loop that the game keeps repeating, acting like the 'motor' of the program
let rec gameLoop((gameBall: ObjectFunctions.GameObject , player1: ObjectFunctions.GameObject , player2 : ObjectFunctions.GameObject)) = async {
    let now = DateTime.Now
    let dt = 64.0 - DateTime.Now.Subtract(now).TotalMilliseconds
    // Apply the function to change the position of the ball to the ball
    let newBall = checkAndLetTheObjectBounce (gameBall, player1, player2) |> moveObject |> perimeter 
    form.KeyDown |> Observable.add morphMutableDirVariable
    form.KeyUp |> Observable.add stopThePaddle
    let newPlayer1 = combiningPlayerAndDirection(player1) |> moveObject
    let playerWithIncrementedColour = checkToChangeColourOfPaddle (gameBall, newPlayer1)
    draw newBall
    drawPlayers (newPlayer1, player2)

   // do! Async.Sleep(Math.Max(dt, 0.0) |> int)
    return! gameLoop(newBall, playerWithIncrementedColour, player2) }

let gameBall = {pos = P((WIDTH/2)-5,(HEIGHT/2)-5); dir = startingAngle(Direction(5.0,5.0), 30.0) ; width = 10 ; height = 10 ; currentColour = 0   }

let player1 =  { pos = P(50,50) ; dir = Direction(5.0,5.0) ; width = 20 ; height = 40 ; currentColour = 0 }
                                                                        
let player2 = { pos = P(550,250) ; dir = Direction(5.0,5.0) ; width = 20 ; height = 40 ; currentColour = 0 }

[<STAThread>]
do Async.Start(gameLoop(gameBall, player1, player2))
   Application.Run(form)
