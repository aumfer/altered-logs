resource "aws_lambda_function" "template" {
  filename         = "../publish.zip"
  function_name    = "${var.repo_name}-${var.branch_name}-template"
  role             = "arn:aws:iam::002067833750:role/acct-managed/altered-logs-lambda"
  handler          = "Altered.Logs::Altered.Logs.Template.MySampleApiCallLambda::Execute"
  source_code_hash = "${filebase64sha256("../publish.zip")}"
  runtime          = "dotnetcore2.1"
  timeout          = "59"                                                             # srsly amazon should just have a seperate first-run timeout

  tags = "${module.tags.tags}"

  environment {
    variables = "${local.altered_tags}"
  }
}

resource "aws_lb_target_group" "template" {
  name        = "${var.repo_name}-${var.branch_name}-template"
  target_type = "lambda"

  tags = "${module.tags.tags}"
}

resource "aws_lb_target_group_attachment" "template" {
  target_group_arn = "${aws_lb_target_group.template.arn}"
  target_id        = "${aws_lambda_function.template.arn}"
  depends_on       = ["aws_lambda_permission.template_allow_alb"]
}

resource "aws_lb_listener_rule" "template_http_path" {
  listener_arn = "${aws_lb_listener.https.arn}"

  action {
    type             = "forward"
    target_group_arn = "${aws_lb_target_group.template.arn}"
  }

  condition {
    field  = "path-pattern"
    values = ["/lambda/template/*"]
  }
}

resource "aws_lambda_permission" "template_allow_alb" {
  statement_id  = "${var.repo_name}-${var.branch_name}-template-alb"
  action        = "lambda:InvokeFunction"
  function_name = "${aws_lambda_function.template.function_name}"
  principal     = "elasticloadbalancing.amazonaws.com"
  source_arn    = "${aws_lb_target_group.template.arn}"
}
