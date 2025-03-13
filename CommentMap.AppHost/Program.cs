var builder = DistributedApplication.CreateBuilder(args);


var rabbitUsername = builder.AddParameter("rabbit-username", secret: true);
var rabbitPassword = builder.AddParameter("rabbit-password", secret: true);

var rabbitmq = builder.AddRabbitMQ("messaging", rabbitUsername, rabbitPassword)
    .WithImageTag("4.0.7-alpine")
    .WithManagementPlugin()
    .WithDataVolume("messaging_data");


var mailpit = builder
    .AddContainer("mailpit", "axllent/mailpit", "v1.23")
    .WithEndpoint(port: 1025, targetPort: 1025, scheme: "smtp", name: "smtp")
    .WithHttpEndpoint(8025, 8025)
    .WithVolume("mailpit_data", "/data");
var smtpEndpoint = mailpit.GetEndpoint("smtp");


builder.AddProject<Projects.CommentMap_EmailSender>("email-sender")
    .WithReference(rabbitmq)
    .WaitFor(rabbitmq)
    .WithReference(smtpEndpoint);


var pgUsername = builder.AddParameter("pg-username", secret: true);
var pgPassword = builder.AddParameter("pg-password", secret: true);

var postgres = builder.AddPostgres("postgres", pgUsername, pgPassword)
    .WithImage("postgis/postgis", "17-3.5-alpine")
    .WithPgAdmin(o => o.WithImageTag("9.1"))
    .WithDataVolume("comment-map_data");
var commentMapDb = postgres.AddDatabase("comment-map");


builder.AddProject<Projects.CommentMap_Mvc>("mvc")
    .WithReference(rabbitmq)
    .WaitFor(rabbitmq)
    .WithReference(commentMapDb)
    .WaitFor(commentMapDb);


builder.Build().Run();
