﻿using FubuMVC.Authentication;
using FubuMVC.Authentication.Membership;
using FubuMVC.Core.Continuations;
using FubuSaml2.Validation;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Saml2.Testing
{
    [TestFixture]
    public class SamlDirectorTester : InteractionContext<SamlDirector>
    {
        [Test]
        public void default_result()
        {
            var result = ClassUnderTest.Result();
            result.Success.ShouldBeFalse();
            result.Continuation.AssertWasRedirectedTo(new LoginRequest
            {
                Message = SamlValidationKeys.UnableToValidationSamlResponse.ToString()
            },"GET");
        }

        [Test]
        public void failed_with_no_continuation()
        {
            ClassUnderTest.FailedUser();

            MockFor<IAuthenticationSession>().AssertWasNotCalled(x => x.MarkAuthenticated("anything"), x => x.IgnoreArguments());

            var result = ClassUnderTest.Result();
            result.Success.ShouldBeFalse();
            result.Continuation.AssertWasRedirectedTo(new LoginRequest
            {
                Message = SamlValidationKeys.UnableToValidationSamlResponse.ToString()
            },"GET");
        }

        [Test]
        public void failed_with_a_continuation()
        {
            var continuation = FubuContinuation.RedirectTo("something");

            ClassUnderTest.FailedUser(continuation);

            var result = ClassUnderTest.Result();
            result.Success.ShouldBeFalse();

            result.Continuation.ShouldBeTheSameAs(continuation);
        }


    }

    [TestFixture]
    public class when_director_succceeds_with_no_continuation : InteractionContext<SamlDirector>
    {
        private const string theUserName = "somebody";
        private FubuPrincipal principal;
        private AuthResult theResult;

        protected override void beforeEach()
        {
            principal = new FubuPrincipal(new UserInfo { UserName = theUserName });
            MockFor<IPrincipalBuilder>().Stub(x => x.Build(theUserName)).Return(principal);

            ClassUnderTest.SuccessfulUser(theUserName);

            theResult = ClassUnderTest.Result();
        }

        [Test]
        public void should_mark_the_session_as_authenticated()
        {
            MockFor<IAuthenticationSession>().AssertWasCalled(x => x.MarkAuthenticated(theUserName));
        }

        [Test]
        public void should_have_set_the_principal()
        {
            MockFor<IPrincipalContext>().AssertWasCalled(x => x.Current = principal);
        }

        [Test]
        public void the_result_is_successful()
        {
            theResult.Success.ShouldBeTrue();
        }

        [Test]
        public void the_continuation_is_to_just_continue()
        {
            theResult.Continuation.AssertWasContinuedToNextBehavior();
        }
    }

    [TestFixture]
    public class when_director_succceeds_with_an_explicit_continuation : InteractionContext<SamlDirector>
    {
        private const string theUserName = "somebody";
        private FubuPrincipal principal;
        private AuthResult theResult;
        private FubuContinuation theContinuation;

        protected override void beforeEach()
        {
            principal = new FubuPrincipal(new UserInfo { UserName = theUserName });
            MockFor<IPrincipalBuilder>().Stub(x => x.Build(theUserName)).Return(principal);

            theContinuation = FubuContinuation.RedirectTo("something");

            ClassUnderTest.SuccessfulUser(theUserName, theContinuation);

            theResult = ClassUnderTest.Result();
        }

        [Test]
        public void should_mark_the_session_as_authenticated()
        {
            MockFor<IAuthenticationSession>().AssertWasCalled(x => x.MarkAuthenticated(theUserName));
        }

        [Test]
        public void should_have_set_the_principal()
        {
            MockFor<IPrincipalContext>().AssertWasCalled(x => x.Current = principal);
        }

        [Test]
        public void the_result_is_successful()
        {
            theResult.Success.ShouldBeTrue();
        }

        [Test]
        public void uses_the_explicit_continuation()
        {
            theResult.Continuation.ShouldBeTheSameAs(theContinuation);
        }
    }
}