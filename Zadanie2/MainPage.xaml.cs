using Newtonsoft.Json;

namespace Zadanie2
{
    public partial class MainPage : ContentPage
    {
        private List<QuizQuestion> quizQuestions;
        private int currentQuestionIndex;
        private string ?jsonContent;
        private string[] userAnswers;

        // Quiz Questions Class to define the structure of the questions
        public class QuizQuestion
        {
            [JsonProperty("question")]
            public string Question { get; set; }

            [JsonProperty("options")]
            public List<string> Options { get; set; }

            [JsonProperty("correct_answer")]
            public string CorrectAnswer { get; set; }
        }


        public MainPage() // Initialization
        {
            Console.WriteLine("\nAPPLICATION START\n");

            InitializeComponent();
            quizQuestions = LoadQuizQuestions("C:\\Users\\jamix\\source\\repos\\Zadanie2\\Zadanie2\\questions.json");

            // Initialize userAnswers array with the same size as the number of quiz questions
            userAnswers = new string[quizQuestions.Count];

            Console.WriteLine("\nAPPLICATION INITIALIZED\n");
        }

        // Start Button Handler
        private void StartButton_Clicked(object sender, EventArgs e)
        {
            LoadQuestion();

            // Debug: Check the correct loading of the questions from json file
            // DisplayAlert("", jsonContent, "OK");

            QuestionsFrame.IsVisible = true;
            StartButton.IsVisible = false; StartButton.IsEnabled = false;
            Choises.IsEnabled = true; Choises.IsVisible = true;
        }

        // Quiz questions List initializer
        private List<QuizQuestion> LoadQuizQuestions(string filePath)
        {
            try
            {
                jsonContent = File.ReadAllText(filePath);

                // Deserialize JSON content into List<QuizQuestion>
                quizQuestions = JsonConvert.DeserializeObject<Questions>(jsonContent).questions;

                Console.WriteLine("\nQUESTIONS LOADED\n");

                foreach (QuizQuestion question in quizQuestions)
                {
                    Console.WriteLine($"\n{question}\n");
                }

                return quizQuestions;
            }
            catch (Exception ex)
            {
                // Handle exceptions
                Console.WriteLine("\nQUESTIONS NOT LOADED\n");

                DisplayAlert("ERROR!", "Error loading quiz questions!", "OK");

                Console.WriteLine($"\nError loading quiz questions: {ex.Message}\n");
                return null;
            }
        }

        // Question Getter
        private void LoadQuestion()
        {
            if (quizQuestions != null && currentQuestionIndex < quizQuestions.Count)
            {
                // Load the next question
                QuizQuestion currentQuestion = quizQuestions[currentQuestionIndex];

                // Update UI with the current question
                QuestionLabel.Text = currentQuestion.Question;

                // Assuming currentQuestion.Options has exactly 4 options
                AnswerA.Content = currentQuestion.Options[0];
                AnswerB.Content = currentQuestion.Options[1];
                AnswerC.Content = currentQuestion.Options[2];
                AnswerD.Content = currentQuestion.Options[3];

                // Increment the question index for the next question
                currentQuestionIndex++;
            }
            else
            {
                // Quiz Completed, calculate and display results
                NextButton.IsVisible = false;
                Answers.IsVisible = false;
                Resoults.IsVisible = true;
                EndResetGrid.IsVisible = true;
                QuestionLabel.Text = "Quiz Completed!";

                // Calculate the score
                int correctAnswers = CalculateCorrectAnswers();

                // Update the labels with the results
                CorrectAnswersLabel.Text = $"{correctAnswers}";
                FalseAnswersLabel.Text = $"{quizQuestions.Count - correctAnswers}";
                TotalLabel.Text = $"{quizQuestions.Count}";

                // Change the appearance of the Results frame based on the number of correct answers
                if (correctAnswers == quizQuestions.Count)
                {
                    GoodGif.IsVisible = true;
                    Resoults.BackgroundColor = Microsoft.Maui.Graphics.Color.FromHex("#4FCA56"); // All answers are correct (Greenish)
                }
                else if (correctAnswers == 0)
                {
                    BedGif.IsVisible = true;
                    Resoults.BackgroundColor = Microsoft.Maui.Graphics.Color.FromHex("#fb6b62"); // No correct answers (Reddish)
                }
                else
                {
                    CloseEnoughGif.IsVisible = true;
                    Resoults.BackgroundColor = Microsoft.Maui.Graphics.Color.FromHex("#6b62fb"); // Some correct answers (Blueish)
                }
            }

            // Reset selected answers for the next question
            ResetSelectedAnswers();
        }

        // Corect answers calculator for the final score
        private int CalculateCorrectAnswers()
        {
            int correctAnswers = 0;
            for (int i = 0; i < quizQuestions.Count; i++)
            {
                QuizQuestion question = quizQuestions[i];
                // Get the selected answer
                string selectedAnswer = userAnswers[i];
                // Check if the selected answer matches the correct answer
                if (selectedAnswer.Contains(question.CorrectAnswer))
                {
                    correctAnswers++;
                }
            }
            return correctAnswers;
        }

        // Answer from RadiButton Getter
        private string GetSelectedAnswer()
        {
            string selectedAnswer = "";     // init

            foreach (RadioButton answer in new RadioButton[] { AnswerA, AnswerB, AnswerC, AnswerD }) // for each answer in RadioButtons(A/B/C/D) check if the answer isChecked
            {
                if (answer.IsChecked)       //if isChecked
                {
                    selectedAnswer = answer.Content.ToString().Substring(0, 1);    // return the answer + cut the everything except the answer letter
                    break;
                }
            }

            return selectedAnswer;  // return the sellected answer or default if non selected
        }

        private void NextButton_Clicked(object sender, EventArgs e)
        {
            // Save the selected answer before loading the next question
            SaveSelectedAnswer();
            // Load Next
            LoadQuestion();
        }

        // RadiButton resitter
        private void ResetSelectedAnswers()
        {
            AnswerA.IsChecked = false;
            AnswerB.IsChecked = false;
            AnswerC.IsChecked = false;
            AnswerD.IsChecked = false;
        }

        // Coronet Question Answer saver
        private void SaveSelectedAnswer()
        {
            // Get the selected answer for the current question
            string selectedAnswer = GetSelectedAnswer(); // Subtract 1 because currentQuestionIndex is already incremented

            // Save the selected answer to the userAnswers array
            userAnswers[currentQuestionIndex - 1] = selectedAnswer; // Subtract 1 because currentQuestionIndex is already incremented
        }

        private void EndButton_Clicked(object sender, EventArgs e)
        {
            // Close the app
            Application.Current.Quit();
        }

        private void RestartButton_Clicked(object sender, EventArgs e)
        {
            // Reset the current question index to restart the quiz from the beginning
            currentQuestionIndex = 0;

            // Reset userAnswers array to clear previous selections
            Array.Clear(userAnswers, 0, userAnswers.Length);

            // Reload the first question
            LoadQuestion();

            // Hide the results frame if it was visible
            NextButton.IsVisible = true;
            Answers.IsVisible = true;
            Resoults.IsVisible = false;
            EndResetGrid.IsVisible = false;

            GoodGif.IsVisible = false;
            BedGif.IsVisible = false;
            CloseEnoughGif.IsVisible = false;

        }
    }


    public class Questions
    {
        public List<MainPage.QuizQuestion> questions { get; set; }
    }
}
