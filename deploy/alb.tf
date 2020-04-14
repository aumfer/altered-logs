data "aws_s3_bucket" "lb_logs" {
  bucket = "altered-logs-alb"
}

resource "aws_lb" "main" {
  name            = "${var.repo_name}-${var.branch_name}"
  security_groups = ["${aws_security_group.security_group.id}"]
  subnets         = ["${data.aws_subnet_ids.subnets.ids}"]
  internal        = false

  # todo https://docs.aws.amazon.com/elasticloadbalancing/latest/classic/enable-access-logs.html#attach-bucket-policy
  access_logs {
    enabled = true
    bucket  = "${data.aws_s3_bucket.lb_logs.bucket}"
  }

  tags = "${local.altered_tags}"
}

resource "aws_lb_listener" "http" {
  load_balancer_arn = "${aws_lb.main.arn}"
  port              = "80"
  protocol          = "HTTP"

  default_action {
    type = "redirect"

    redirect {
      port        = "443"
      protocol    = "HTTPS"
      status_code = "HTTP_301"
    }
  }
}

resource "aws_lb_listener" "https" {
  load_balancer_arn = "${aws_lb.main.arn}"
  port              = "443"
  protocol          = "HTTPS"
  certificate_arn   = "arn:aws:acm:us-east-1:021370510137:certificate/b5933088-78d3-46b4-bf44-efd582040c14"

  default_action {
    type = "redirect"

    redirect {
      host        = "github.com"
      path        = "/aumfer/altered-logs"
      status_code = "HTTP_302"
    }
  }
}