resource "aws_iam_role" "iam_role" {
    name = "${var.repo_name}-${var.branch_name}-alb"
    tags = "${local.altered_tags}"

    assume_role_policy = "${data.aws_iam_policy_document.iam_lambda_alb_assume_role_policy.json}"
}

data "aws_iam_policy_document" "iam_lambda_alb" {
    statement {
        sid       = "get"
        effect    = "Allow"
        resources = [
            "${data.aws_s3_bucket.alb_logs.id}/",
        ]
        actions = [
            "s3:GetObject"
        ]
    }

    statement {
        sid = "log"
        effect = "Allow"
        resources = [
            "*"
        ]
        actions = [
            # create stream, put event, put metric
            "logs:*",
            "cloudwatch:*"
        ]
    }
}

data "aws_iam_policy_document" "iam_lambda_alb_assume_role_policy" {
    statement {
        sid       = "exec"
        effect    = "Allow"
        actions = [
            "sts:AssumeRole"
        ]

        principals {
            type = "Service"
            # everything
            identifiers = ["lambda.amazonaws.com", "ec2.amazonaws.com", "ecs-tasks.amazonaws.com", "ecs.amazonaws.com", "ec2.application-autoscaling.amazonaws.com", "ecs.application-autoscaling.amazonaws.com"]
        }
    }
}

resource "aws_iam_policy" "iam_lambda_alb" {
  name   = "${var.repo_name}-${var.branch_name}-alb"
  policy = "${data.aws_iam_policy_document.iam_lambda_alb.json}"
}

resource "aws_iam_role_policy_attachment" "iam_lambda_alb" {
  role       = "${aws_iam_role.iam_lambda_alb.name}"
  policy_arn = "${aws_iam_policy.iam_lambda_alb.arn}"
}

resource "aws_iam_role_policy_attachment" "iam_lambda_alb_AmazonECSTaskExecutionRolePolicy" {
  role       = "${aws_iam_role.iam_lambda_alb.name}"
  policy_arn = "arn:aws:iam::aws:policy/service-role/AmazonECSTaskExecutionRolePolicy"
}

resource "aws_iam_role_policy_attachment" "iam_lambda_alb_AmazonEC2ContainerServiceforEC2Role" {
  role       = "${aws_iam_role.iam_lambda_alb.name}"
  policy_arn = "arn:aws:iam::aws:policy/service-role/AmazonEC2ContainerServiceforEC2Role"
}
