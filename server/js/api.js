const token = typeof terminalObject === 'undefined' ? null : terminalObject.getToken();

const callApi = (method, formData = false) => {
  const params = {
    method: 'POST',
    headers: {
      'Authorization': token
    },
    credentials: 'include'
  };

  if (formData && formData instanceof FormData) params.body = formData;

  return new Promise((resolve, reject) => {
    fetch(`https://<YOUR_DOMAIN_NAME>.com/api/${method}`, params)
      .then(response => response.json())
      .then(json => {
        if (json.status == 'success' && json.data !== null) {
          resolve(json);
        } else {
          reject(json.data);
        }
      })
      .catch(error => {
        reject(error);
      });
  });
}
