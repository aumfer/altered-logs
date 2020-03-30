resource "aws_lambda_function" "ecs" {
  filename         = "../publish.zip"
  function_name    = "${var.repo_name}-${var.branch_name}-ecs"
  role             = "arn:aws:iam::002067833750:role/acct-managed/altered-logs-lambda"
  handler          = "Altered.Logs::Altered.Logs.Ecs.LogEcsTaskStateChangeLambda::Execute"
  source_code_hash = "${base64sha256(file("../publish.zip"))}"
  runtime          = "dotnetcore2.1"
  timeout          = "59"                                                              # srsly amazon should just have a seperate first-run timeout

  tags = "${module.tags.tags}"

  environment {
    variables = "${local.altered_tags}"
  }
}

resource "aws_lambda_permission" "ecs_allow_events" {
  statement_id  = "${var.repo_name}-${var.branch_name}-ecs-events"
  action        = "lambda:InvokeFunction"
  function_name = "${aws_lambda_function.ecs.function_name}"
  principal     = "events.amazonaws.com"
}

resource "aws_cloudwatch_event_target" "ecs" {
  target_id = "${var.repo_name}-${var.branch_name}-ecs"
  rule      = "${aws_cloudwatch_event_rule.ecs.name}"
  arn       = "${aws_lambda_function.ecs.arn}"
}

resource "aws_cloudwatch_event_rule" "ecs" {
  name = "${var.repo_name}-${var.branch_name}-ecs"

  event_pattern = <<PATTERN
{
  "source": [
    "aws.ecs"
  ],
  "detail-type": [
    "ECS Task State Change",
    "ECS Container Instance State Change"
  ]
}
PATTERN
}
