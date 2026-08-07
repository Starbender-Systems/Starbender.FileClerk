(() => {
  const defaults = { username: "admin", password: "1q2w3E*" };

  const run = () => {
    if (!/\/Account\/Login\/?$/i.test(window.location.pathname)) return;

    const username = document.querySelector(
      'input[name$="UserNameOrEmailAddress"], input[id$="UserNameOrEmailAddress"]'
    );
    const password = document.querySelector('input[type="password"][name$="Password"]');

    if (username && !username.value) username.value = defaults.username;
    if (password && !password.value) password.value = defaults.password;
  };

  if (document.readyState === "loading") {
    document.addEventListener("DOMContentLoaded", run);
  } else {
    run();
  }
})();
