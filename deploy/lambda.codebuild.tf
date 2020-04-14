resource "aws_lambda_function" "codebuild" {
  filename         = "../publish.zip"
  function_name    = "${var.repo_name}-${var.branch_name}-codebuild"
  role             = "${aws_iam_role.iam.arn}"
  handler          = "Altered.Logs::Altered.Logs.Codebuild.LogCodebuildEventLambda::Execute"
  source_code_hash = "${base64sha256(file("../publish.zip"))}"
  runtime          = "dotnetcore2.1"
  timeout          = "59"                                                              # srsly amazon should just have a seperate first-run timeout

  tags = "${local.altered_tags}"

  environment {
    variables = "${local.altered_tags}"
  }
}

resource "aws_lambda_permission" "codebuild_allow_events" {
  statement_id  = "${var.repo_name}-${var.branch_name}-codebuild-events"
  action        = "lambda:InvokeFunction"
  function_name = "${aws_lambda_function.codebuild.function_name}"
  principal     = "events.amazonaws.com"
}

resource "aws_cloudwatch_event_target" "codebuild" {
  target_id = "${var.repo_name}-${var.branch_name}-codebuild"
  rule      = "${aws_cloudwatch_event_rule.codebuild.name}"
  arn       = "${aws_lambda_function.codebuild.arn}"
}

resource "aws_cloudwatch_event_rule" "codebuild" {
  name = "${var.repo_name}-${var.branch_name}-codebuild"

  event_pattern = <<PATTERN
{
  "source": [
    "aws.codebuild"
  ],
  "detail-type": [
    "CodeBuild Build State Change",
	  "CodeBuild Build Phase Change"
  ]
}
PATTERN
}
