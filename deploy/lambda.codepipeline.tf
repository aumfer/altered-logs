resource "aws_lambda_function" "codepipeline" {
  filename         = "../publish.zip"
  function_name    = "${var.repo_name}-${var.branch_name}-codepipeline"
  role             = "${aws_iam_role.iam.arn}"
  handler          = "Altered.Logs::Altered.Logs.CodePipeline.LogCodePipelineEventLambda::Execute"
  source_code_hash = "${base64sha256(file("../publish.zip"))}"
  runtime          = "dotnetcore2.1"
  #timeout          = "59"                                                              # srsly amazon should just have a seperate first-run timeout
  timeout = "300" # for slack

  tags = "${local.altered_tags}"

  environment {
      variables = "${merge(local.altered_tags, map("ELASTICSEARCH_URL", "https://${aws_elasticsearch_domain.search.endpoint}"))}"
  }
}

resource "aws_lambda_permission" "codepipeline_allow_events" {
  statement_id  = "${var.repo_name}-${var.branch_name}-codepipeline-events"
  action        = "lambda:InvokeFunction"
  function_name = "${aws_lambda_function.codepipeline.function_name}"
  principal     = "events.amazonaws.com"
}

resource "aws_cloudwatch_event_target" "codepipeline" {
  target_id = "${var.repo_name}-${var.branch_name}-codepipeline"
  rule      = "${aws_cloudwatch_event_rule.codepipeline.name}"
  arn       = "${aws_lambda_function.codepipeline.arn}"
}

resource "aws_cloudwatch_event_rule" "codepipeline" {
  name = "${var.repo_name}-${var.branch_name}-codepipeline"

  event_pattern = <<PATTERN
{
  "source": [
    "aws.codepipeline"
  ],
  "detail-type": [
    "CodePipeline Pipeline Execution State Change",
	  "CodePipeline Stage Execution State Change",
	  "CodePipeline Action Execution State Change"
  ]
}
PATTERN
}
