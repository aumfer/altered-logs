resource "aws_lambda_function" "asg" {
  filename         = "../publish.zip"
  function_name    = "${var.repo_name}-${var.branch_name}-asg"
  role             = "${aws_iam_role.iam.arn}"
  handler          = "Altered.Logs::Altered.Logs.Autoscaling.LogAutoscalingEventLambda::Execute"
  source_code_hash = "${base64sha256(file("../publish.zip"))}"
  runtime          = "dotnetcore2.1"
  timeout          = "59"                                                              # srsly amazon should just have a seperate first-run timeout

  tags = "${local.altered_tags}"

  environment {
    variables = "${local.altered_tags}"
  }
}

resource "aws_lambda_permission" "asg_allow_events" {
  statement_id  = "${var.repo_name}-${var.branch_name}-asg-events"
  action        = "lambda:InvokeFunction"
  function_name = "${aws_lambda_function.asg.function_name}"
  principal     = "events.amazonaws.com"
}

resource "aws_cloudwatch_event_target" "asg" {
  target_id = "${var.repo_name}-${var.branch_name}-asg"
  rule      = "${aws_cloudwatch_event_rule.asg.name}"
  arn       = "${aws_lambda_function.asg.arn}"
}

resource "aws_cloudwatch_event_rule" "asg" {
  name = "${var.repo_name}-${var.branch_name}-asg"

  event_pattern = <<PATTERN
{
  "source": [
    "aws.autoscaling"
  ],
  "detail-type": [
    "EC2 Instance Launch Successful",
    "EC2 Instance Terminate Successful",
    "EC2 Instance Launch Unsuccessful",
    "EC2 Instance Terminate Unsuccessful",
    "EC2 Instance-launch Lifecycle Action",
    "EC2 Instance-terminate Lifecycle Action"
  ]
}
PATTERN
}
