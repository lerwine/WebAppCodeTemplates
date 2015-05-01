SELECT * FROM OpenQuery ( 
  ADSI,  
  'SELECT displayName, telephoneNumber, mail, mobile, facsimileTelephoneNumber 
  FROM  ''LDAP://DOMAIN.com/OU=Players,DC=DOMAIN,DC=com'' 
  WHERE objectClass =  ''User'' 
  ') AS tblADSI
ORDER BY displayname