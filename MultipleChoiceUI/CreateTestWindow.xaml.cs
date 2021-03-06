﻿using System;
using System.Windows;
using System.Windows.Controls;
using MultipleChoiceLibrary;
using MultipleChoiceUI.User_Controls;

namespace MultipleChoiceUI
{
    /// <summary>
    /// Interaction logic for CreateTestWindow.xaml
    /// </summary>
    public partial class CreateTestWindow
    {
        private readonly int _testId;
        private int _userId;

        public CreateTestWindow(int userId)
        {
            InitializeComponent();

            Test test = new Test { Name = "" };
            TestController.AddTest(test);
            _testId = test.Test_ID;
            _userId = userId;
        }

        //  If we are editing a test we need the test ID
        public CreateTestWindow(int testId, int userId)
        {
            InitializeComponent();
            _testId = testId;
            _userId = userId;
            Heading.Text = $"{TestController.GetTestName(_testId)}";
            TestTotal.Text = $"Total Marks: {TestController.GetTotalMarks(_testId)}";
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            RefreshQuestions();
        }

        /// <summary>
        /// Refreshes all the question user controls
        /// </summary>
        private void RefreshQuestions()
        {
            TestTotal.Text = $"Total Marks: {TestController.GetTotalMarks(_testId)}";
            StackPanel.Children.Clear();

            var questions = TestController.GetQuestions(_testId);
            for (int i = 0; i < questions.Count; i++)
            {
                QuestionUserControl question = GetUserControl(questions[i], i);
                StackPanel.Children.Add(question);
            }
        }

        /// <summary>
        /// Displays a dialog to edit / create a new question
        /// </summary>
        private void AddQuestion_Click(object sender, RoutedEventArgs e)
        {
            EditQuestionWindow editQuestion = new EditQuestionWindow(_testId)
            {
                Owner = this,
            };

            Effect = Utility.Blur;
            Opacity = 0.5;
            editQuestion.ShowDialog();
            RefreshQuestions();
        }

        /// <summary>
        /// Submits the completed test
        /// </summary>
        private void SubmitTest_Click(object sender, RoutedEventArgs e)
        {
            if (TestController.GetQuestions(_testId).Count == 0)
            {
                MessageBox.Show("Please have at least 1 question");
                return;
            }

            SubmitTestWindow submitTest = new SubmitTestWindow
            {
                SelectedTestName = TestController.GetTestName(_testId),
                Owner = this
            };
            Effect = Utility.Blur;
            submitTest.ShowDialog();

            if (submitTest.DialogResult == true)
            {
                TestController.SetTestName(_testId, submitTest.SelectedTestName);
                LectureMenuWindow lectureMenu = new LectureMenuWindow(_userId);
                lectureMenu.Show();
                Close();
            }
        }

        /// <summary>
        /// Creates a new user control
        /// </summary>
        /// <param name="question">The question to add to the user control</param>
        /// <param name="i">The question number</param>
        private QuestionUserControl GetUserControl(Question question, int i)
        {
            QuestionUserControl questionUserControl = new QuestionUserControl();

            //  Initialize the controls
            questionUserControl.InitControls(question, i);

            //  Subscribe the edit and delete button to the button click event
            questionUserControl.Edit.Click += Button_Click_Edit;
            questionUserControl.Edit.Tag = question.Question_ID;

            questionUserControl.Delete.Click += Button_Click_Delete;
            questionUserControl.Delete.Tag = question.Question_ID;

            return questionUserControl;
        }

        /// <summary>
        /// Called when any edit button is clicked
        /// </summary>
        private void Button_Click_Edit(object sender, EventArgs e)
        {
            Button button = sender as Button;
            if (button?.Tag == null) return;

            int questionId = (int)button.Tag;

            LoadEditQuestionForm(questionId);
        }

        /// <summary>
        /// Called when any delete button is clicked
        /// </summary>
        private void Button_Click_Delete(object sender, EventArgs e)
        {
            Button button = sender as Button;
            if (button?.Tag == null) return;

            int questionId = (int)button.Tag;

            MessageBoxResult dialogResult = MessageBox.Show(
                "Are You Sure?",
                "Delete Question",
                MessageBoxButton.YesNo);

            if (dialogResult != MessageBoxResult.Yes) return;

            TestController.RemoveQuestion(questionId);
            RefreshQuestions();
        }

        /// <summary>
        /// Loads the form to edit the question that the user requested
        /// </summary>
        private void LoadEditQuestionForm(int questionId)
        {
            EditQuestionWindow editQuestion = new EditQuestionWindow(_testId, questionId);
            editQuestion.Owner = this;
            editQuestion.Effect = Utility.DropShadowEffect;

            Effect = Utility.Blur;
            editQuestion.ShowDialog();
            RefreshQuestions();
        }

        /// <summary>
        /// Expands all the questions
        /// </summary>
        private void ExpandAll_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < StackPanel.Children.Count; i++)
            {
                if (StackPanel.Children[i] is QuestionUserControl userControl)
                {
                    userControl.Expander.IsExpanded = true;
                }
            }
        }

        /// <summary>
        /// Collapses all the questions
        /// </summary>
        private void CollapseAll_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < StackPanel.Children.Count; i++)
            {
                if (StackPanel.Children[i] is QuestionUserControl userControl)
                {
                    userControl.Expander.IsExpanded = false;
                }
            }
        }
    }
}
